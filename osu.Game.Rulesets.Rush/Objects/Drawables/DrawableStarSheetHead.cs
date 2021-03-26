// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableStarSheetHead : DrawableStarSheetCap<StarSheetHead>
    {
        protected override RushSkinComponents Component => RushSkinComponents.StarSheetHead;

        public DrawableStarSheetHead(DrawableStarSheet starSheet)
            : base(starSheet, starSheet.HitObject.Head)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered)
            {
                var result = HitObject.HitWindows.ResultFor(timeOffset);
                if (result == HitResult.None)
                    return;

                ApplyResult(r => r.Type = result);
            }
            else if (!HitObject.HitWindows.CanBeHit(timeOffset))
            {
                ApplyResult(r => r.Type = r.Judgement.MinResult);
            }
        }

        protected override void AdjustAnchor()
        {
            if (HitObject is null) return;
            Anchor = LeadingAnchor;
        }
    }
}
