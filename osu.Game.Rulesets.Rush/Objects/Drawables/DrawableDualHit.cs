// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Scoring;
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

        public DrawableDualHit()
        : this(null)
        { }

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
            airHitContainer.Clear(false);
            groundHitContainer.Clear(false);
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

        public override bool OnPressed(RushAction action)
        {
            if (AllJudged)
                return false;

            var airResult = !Air.AllJudged && Air.LaneMatchesAction(action) && Air.UpdateResult(true);
            var groundResult = !Ground.AllJudged && Ground.LaneMatchesAction(action) && Ground.UpdateResult(true);

            if (Air.AllJudged && Ground.AllJudged)
                return UpdateResult(true);

            return airResult || groundResult;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged)
                return;

            if (!userTriggered && timeOffset < 0)
                return;

            if (!Ground.AllJudged)
                Ground.UpdateResult(false);

            if (!Air.AllJudged)
                Air.UpdateResult(false);

            if (Air.Result.Type == HitResult.None || Ground.Result.Type == HitResult.None)
                return;

            // If we missed either, it's an overall miss.
            // If we hit both, the overall judgement is the lowest score of the two.
            ApplyResult(r =>
            {
                var lowestResult = Air.Result.Type < Ground.Result.Type ? Air.Result.Type : Ground.Result.Type;
                r.Type = !Air.IsHit && !Ground.IsHit ? r.Judgement.MinResult : lowestResult;
            });
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
