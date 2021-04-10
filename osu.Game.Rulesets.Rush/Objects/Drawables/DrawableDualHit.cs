// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableDualHit : DrawableRushHitObject<DualHit>
    {
        private readonly Container<DrawableDualHitPart> airHitContainer;
        private readonly Container<DrawableDualHitPart> groundHitContainer;
        private readonly SkinnableDrawable skinnedJoin;

        public DrawableDualHitPart Air => airHitContainer.Child;
        public DrawableDualHitPart Ground => groundHitContainer.Child;

        public DrawableDualHit(DualHit hitObject)
            : base(hitObject)
        {
            RelativeSizeAxes = Axes.Y;
            Height = 1f;

            Content.AddRange(new Drawable[]
            {
                skinnedJoin = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.DualHitJoin), _ => new DualHitJoinPiece()),
                airHitContainer = new Container<DrawableDualHitPart> { RelativeSizeAxes = Axes.Both },
                groundHitContainer = new Container<DrawableDualHitPart> { RelativeSizeAxes = Axes.Both },
            });
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableDualHitPart part:
                    (part.HitObject.Lane == LanedHitLane.Air ? airHitContainer : groundHitContainer).Add(part);
                    break;
            }
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            airHitContainer.Clear();
            groundHitContainer.Clear();
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case DualHitPart part:
                    return new DrawableDualHitPart(part);
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            skinnedJoin.Show();
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            Origin = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
        }

        // Input are handled by the DualHit parts, since they still act like normal minions
        public override bool OnPressed(RushAction action) => false;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged) return;

            // We can judge this object the instant the nested objects are judged
            if (Air.AllJudged && Ground.AllJudged)
                ApplyResult(r => r.Type = (Air.IsHit && Ground.IsHit) ? r.Judgement.MaxResult : r.Judgement.MinResult);
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            const float animation_time = 300f;

            switch (state)
            {
                case ArmedState.Miss:
                    this.FadeOut(animation_time);
                    break;

                case ArmedState.Hit:
                    ProxyContent();
                    skinnedJoin.Hide();
                    this.FadeOut(animation_time);
                    break;
            }
        }
    }
}
