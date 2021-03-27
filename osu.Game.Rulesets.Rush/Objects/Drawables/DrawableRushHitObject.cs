// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Bindings;
using osu.Framework.Utils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public abstract class DrawableRushHitObject : DrawableHitObject<RushHitObject>, IKeyBindingHandler<RushAction>
    {
        public static readonly Color4 AIR_ACCENT_COLOUR = new Color4(0.35f, 0.75f, 1f, 1f);
        public static readonly Color4 GROUND_ACCENT_COLOUR = new Color4(1f, 0.4f, 1f, 1f);
        public static readonly float LIFETIME_END_DELAY = 500f;

        protected readonly Container Content;
        private readonly Container proxiedContent;

        private readonly Container nonProxiedContent;

        protected readonly IBindable<ScrollingDirection> Direction = new Bindable<ScrollingDirection>();

        public override double LifetimeStart
        {
            get => base.LifetimeStart;
            set
            {
                base.LifetimeStart = value;
                proxiedContent.LifetimeStart = value;
            }
        }

        public override double LifetimeEnd
        {
            get => base.LifetimeEnd;
            set
            {
                base.LifetimeEnd = value;
                proxiedContent.LifetimeEnd = value;
            }
        }

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

        protected DrawableRushHitObject(RushHitObject hitObject)
            : base(hitObject)
        {
            AddRangeInternal(new[]
            {
                nonProxiedContent = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Child = Content = new Container { RelativeSizeAxes = Axes.Both }
                },
                proxiedContent = new ProxiedContentContainer { RelativeSizeAxes = Axes.Both }
            });
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

        //protected override void UpdateStartTimeStateTransforms() => UnproxyContent();

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Miss:
                    AccentColour.Value = Color4.Gray;
                    //ProxyContent();

                    if (!ExpireOnMiss)
                        LifetimeEnd = HitObject.GetEndTime() + LifetimeEndDelay;

                    break;

                case ArmedState.Hit:
                    //ProxyContent();

                    if (!ExpireOnHit)
                        LifetimeEnd = HitObject.GetEndTime() + LifetimeEndDelay;

                    break;
            }
        }

        public virtual Drawable CreateHitExplosion() => new DefaultHitExplosion(Color4.Yellow.Darken(0.5f))
        {
            Anchor = Anchor,
            Origin = Anchor.Centre,
            Size = new Vector2(200, 200),
            Scale = new Vector2(0.9f + RNG.NextSingle() * 0.2f),
            Rotation = RNG.NextSingle() * 360f,
        };

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
        }

        #region Proxying logic

        private bool isProxied;

        protected void ProxyContent()
        {
            if (isProxied) return;

            isProxied = true;

            nonProxiedContent.Remove(Content);
            proxiedContent.Add(Content);
        }

        protected void UnproxyContent()
        {
            if (!isProxied) return;

            isProxied = false;

            proxiedContent.Remove(Content);
            nonProxiedContent.Add(Content);
        }

        public Drawable CreateProxiedContent() => proxiedContent.CreateProxy();

        private class ProxiedContentContainer : Container
        {
            public override bool RemoveWhenNotAlive => false;
        }

        #endregion
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
