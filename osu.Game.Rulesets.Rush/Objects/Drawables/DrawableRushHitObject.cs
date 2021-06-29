// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Rush.UI.Fever;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public abstract class DrawableRushHitObject : DrawableHitObject<RushHitObject>, IKeyBindingHandler<RushAction>
    {
        public static readonly Color4 AIR_ACCENT_COLOUR = new Color4(0.35f, 0.75f, 1f, 1f);
        public static readonly Color4 GROUND_ACCENT_COLOUR = new Color4(1f, 0.4f, 1f, 1f);
        public static readonly float LIFETIME_END_DELAY = 500f;

        public Func<DrawableHitObject, bool> CheckHittable;

        protected readonly IBindable<ScrollingDirection> Direction = new Bindable<ScrollingDirection>();

        protected override bool ComputeIsMaskedAway(RectangleF maskingBounds) => false;

        protected RushInputManager ActionInputManager => (RushInputManager)GetContainingInputManager();

        /// <summary>
        /// Whether to display an explosion when this hit object is hit.
        /// </summary>
        public virtual bool DisplayExplosion => false;

        protected virtual bool ExpireOnHit => true;
        protected virtual bool ExpireOnMiss => false;
        protected virtual float LifetimeEndDelay => LIFETIME_END_DELAY;

        [Resolved]
        private DrawableRushRuleset drawableRuleset { get; set; }

        [Resolved]
        private FeverProcessor feverProcessor { get; set; }

        private readonly Container<DrawableFeverBonus> feverBonusContainer;

        protected DrawableRushHitObject(RushHitObject hitObject)
            : base(hitObject)
        {
            AddInternal(feverBonusContainer = new Container<DrawableFeverBonus>());
        }

        [BackgroundDependencyLoader(true)]
        private void load([NotNull] IScrollingInfo scrollingInfo)
        {
            Direction.BindTo(scrollingInfo.Direction);
            Direction.BindValueChanged(OnDirectionChanged, true);
        }

        protected virtual void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            Origin = Anchor.Centre;
            Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (timeOffset >= 0)
                ApplyResult(r => r.Type = r.Judgement.MaxResult);
        }

        public virtual bool OnPressed(RushAction action) => false;

        public virtual void OnReleased(RushAction action)
        {
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Miss:
                    AccentColour.Value = Color4.Gray;

                    if (!ExpireOnMiss)
                        LifetimeEnd = HitObject.GetEndTime() + LifetimeEndDelay;

                    break;

                case ArmedState.Hit:
                    if (!ExpireOnHit)
                        LifetimeEnd = HitObject.GetEndTime() + LifetimeEndDelay;

                    break;
            }
        }

        protected override JudgementResult CreateResult(Judgement judgement) => new RushJudgementResult(HitObject, (RushJudgement)judgement);

        protected new void ApplyResult(Action<JudgementResult> application)
        {
            // This is the only point to correctly apply values to the judgement
            // result in correct time, check whether the player collided now.
            Action<JudgementResult> rushApplication = br =>
            {
                var r = (RushJudgementResult)br;

                application?.Invoke(r);
                r.PlayerCollided = drawableRuleset.PlayerCollidesWith(r.HitObject);
            };

            base.ApplyResult(rushApplication);

            foreach (var bonus in feverBonusContainer)
            {
                bool eligible = IsHit && feverProcessor.InFeverMode.Value;
                bonus.ApplyResult(result => result.Type = eligible ? result.Judgement.MaxResult : result.Judgement.MinResult);
            }
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            if (hitObject is FeverBonus x)
                return new DrawableFeverBonus(x);

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            if (hitObject is DrawableFeverBonus x)
                feverBonusContainer.Add(x);

            base.AddNestedHitObject(hitObject);
        }

        protected override void ClearNestedHitObjects()
        {
            feverBonusContainer.Clear(false);
            base.ClearNestedHitObjects();
        }
    }

    public abstract class DrawableRushHitObject<TObject> : DrawableRushHitObject
        where TObject : RushHitObject
    {
        public new TObject HitObject => (TObject)base.HitObject;

        protected DrawableRushHitObject(TObject hitObject)
            : base(hitObject)
        {
        }
    }
}
