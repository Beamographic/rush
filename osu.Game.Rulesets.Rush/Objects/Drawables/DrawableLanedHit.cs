// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableLanedHit<TLanedHit> : DrawableRushHitObject<TLanedHit>, IDrawableLanedHit
        where TLanedHit : LanedHit
    {
        public virtual Color4 LaneAccentColour => HitObject.Lane == LanedHitLane.Air ? AIR_ACCENT_COLOUR : GROUND_ACCENT_COLOUR;

        public Anchor LaneAnchor =>
            HitObject.Lane switch
            {
                LanedHitLane.Air => Direction.Value == ScrollingDirection.Left ? Anchor.TopLeft : Anchor.TopRight,
                LanedHitLane.Ground => Direction.Value == ScrollingDirection.Left ? Anchor.BottomLeft : Anchor.BottomRight,
                _ => Direction.Value == ScrollingDirection.Left ? Anchor.BottomLeft : Anchor.BottomRight
            };

        public Anchor LeadingAnchor => Direction.Value == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;

        public Anchor TrailingAnchor => Direction.Value == ScrollingDirection.Left ? Anchor.CentreRight : Anchor.CentreLeft;

        public LanedHitLane Lane => HitObject?.Lane ?? LanedHitLane.Air;

        public DrawableLanedHit(TLanedHit hitObject)
            : base(hitObject)
        {
        }

        protected override void OnApply()
        {
            base.OnApply();
            AdjustAnchor();
            AccentColour.Value = LaneAccentColour;
        }

        protected virtual void AdjustAnchor()
        {
            if (HitObject is null) return;
            Anchor = LaneAnchor;
        }


        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);
            AdjustAnchor();
        }

        public override bool OnPressed(RushAction action)
        {
            if (!LaneMatchesAction(action))
                return false;

            return UpdateResult(true);
        }

        public virtual bool LaneMatchesAction(RushAction action) => action.Lane() == HitObject.Lane;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = r.Judgement.MinResult);
                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);
            if (result == HitResult.None)
                return;

            ApplyResult(r => r.Type = result);
        }

        protected override void UpdateStartTimeStateTransforms()
        {
            base.UpdateStartTimeStateTransforms();
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            switch (state)
            {
                case ArmedState.Miss:
                    AccentColour.Value = Color4.Gray;
                    break;
            }
        }
    }
}
