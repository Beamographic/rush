// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableLanedHit<TLanedHit> : DrawableDashHitObject<TLanedHit>
        where TLanedHit : LanedHit
    {
        private bool validActionPressed;

        public DrawableLanedHit(TLanedHit hitObject)
            : base(hitObject)
        {
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            if (HitObject.Lane == LanedHitLane.Air)
                Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.TopLeft : Anchor.TopRight;
            else
                Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.BottomLeft : Anchor.BottomRight;
        }

        public override bool OnPressed(DashAction action)
        {
            if (!LaneMatchesAction(action))
                return false;

            return UpdateResult(true);
        }

        protected virtual bool LaneMatchesAction(DashAction action)
        {
            switch (HitObject.Lane)
            {
                case LanedHitLane.Air:
                    return action == DashAction.AirPrimary || action == DashAction.AirSecondary;

                case LanedHitLane.Ground:
                    return action == DashAction.GroundPrimary || action == DashAction.GroundSecondary;
            }

            return false;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);
                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);
            if (result == HitResult.None)
                return;

            // if (!validActionPressed)
            //     ApplyResult(r => r.Type = HitResult.Miss);
            // else
            ApplyResult(r => r.Type = result);
        }
    }
}
