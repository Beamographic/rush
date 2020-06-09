// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableDualHit : DrawableRushHitObject<DualHit>
    {
        private readonly Container<DrawableDualHitPart> airHitContainer;
        private readonly Container<DrawableDualHitPart> groundHitContainer;
        private readonly Box joinBox;

        public DrawableDualHitPart Air => airHitContainer.Child;
        public DrawableDualHitPart Ground => groundHitContainer.Child;

        public DrawableDualHit(DualHit hitObject)
            : base(hitObject)
        {
            RelativeSizeAxes = Axes.Y;
            Height = 1f;

            Content.AddRange(new Drawable[]
            {
                joinBox = new Box
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Y,
                    Size = new Vector2(10f, 1f)
                },
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

            joinBox.Colour = ColourInfo.GradientVertical(AIR_ACCENT_COLOUR, GROUND_ACCENT_COLOUR);
            joinBox.Show();
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

            if (!Air.AllJudged && Air.LaneMatchesAction(action))
                Air.UpdateResult(true);
            else if (!Ground.AllJudged && Ground.LaneMatchesAction(action))
                Ground.UpdateResult(true);
            else
                return false;

            return Air.AllJudged && Ground.AllJudged && UpdateResult(true);
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

                if (!Air.IsHit && !Ground.IsHit)
                    r.Type = HitResult.Miss;
                else
                    r.Type = lowestResult;
            });
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            const float animation_time = 300f;

            switch (state)
            {
                case ArmedState.Idle:
                    UnproxyContent();
                    break;

                case ArmedState.Miss:
                    this.FadeOut(animation_time);
                    break;

                case ArmedState.Hit:
                    ProxyContent();
                    joinBox.Hide();
                    this.FadeOut(animation_time);
                    break;
            }
        }
    }
}
