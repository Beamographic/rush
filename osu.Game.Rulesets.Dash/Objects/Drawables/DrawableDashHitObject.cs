// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public abstract class DrawableDashHitObject : DrawableHitObject<DashHitObject>, IKeyBindingHandler<DashAction>
    {
        protected readonly Container Content;
        private readonly Container proxiedContent;

        private readonly Container nonProxiedContent;

        protected readonly IBindable<ScrollingDirection> Direction = new Bindable<ScrollingDirection>();

        protected DrawableDashHitObject(DashHitObject hitObject)
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

        protected override void UpdateStateTransforms(ArmedState state)
        {
            const double duration = 150;

            switch (state)
            {
                case ArmedState.Hit:
                    this.FadeOut(duration, Easing.OutQuint).Expire();
                    break;

                case ArmedState.Miss:

                    this.FadeColour(Color4.Red, duration);
                    this.FadeOut(duration, Easing.InQuint).Expire();
                    break;
            }
        }

        public virtual bool OnPressed(DashAction action) => false;

        public virtual void OnReleased(DashAction action)
        {
        }

        private class ProxiedContentContainer : Container
        {
            public override bool RemoveWhenNotAlive => false;
        }
    }

    public abstract class DrawableDashHitObject<TObject> : DrawableDashHitObject
        where TObject : DashHitObject
    {
        public new readonly TObject HitObject;

        protected DrawableDashHitObject(TObject hitObject)
            : base(hitObject)
        {
            HitObject = hitObject;
        }
    }
}
