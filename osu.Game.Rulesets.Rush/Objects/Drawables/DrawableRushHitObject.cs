// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
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

        protected RushInputManager ActionInputManager => (RushInputManager)GetContainingInputManager();

        protected virtual bool ExpireOnHit => true;
        protected virtual bool ExpireOnMiss => false;

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

        protected override bool ComputeIsMaskedAway(RectangleF maskingBounds) => false;

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
                // todo: implement judgement logic
                ApplyResult(r => r.Type = HitResult.Perfect);
        }

        public virtual bool OnPressed(RushAction action) => false;

        public virtual void OnReleased(RushAction action)
        {
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            UnproxyContent();
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Idle:
                    UnproxyContent();
                    break;

                case ArmedState.Miss:
                    AccentColour.Value = Color4.Gray;
                    ProxyContent();

                    if (!ExpireOnMiss)
                        LifetimeEnd = HitObject.GetEndTime() + LIFETIME_END_DELAY;

                    break;

                case ArmedState.Hit:
                    ProxyContent();

                    if (!ExpireOnHit)
                        LifetimeEnd = HitObject.GetEndTime() + LIFETIME_END_DELAY;

                    break;
            }
        }

        private class ProxiedContentContainer : Container
        {
            public override bool RemoveWhenNotAlive => false;
        }
    }

    public abstract class DrawableRushHitObject<TObject> : DrawableRushHitObject
        where TObject : RushHitObject
    {
        public new readonly TObject HitObject;

        protected DrawableRushHitObject(TObject hitObject)
            : base(hitObject)
        {
            HitObject = hitObject;
        }
    }
}
