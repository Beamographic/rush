// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public partial class DrawableStarSheetHead : DrawableStarSheetCap<StarSheetHead>
    {
        protected override RushSkinComponents Component => RushSkinComponents.StarSheetHead;

        protected override Anchor CapAnchor => LeadingAnchor;

        public DrawableStarSheetHead()
            : this(null)
        {
        }

        public DrawableStarSheetHead(StarSheetHead starSheetHead)
            : base(starSheetHead)
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
    }
}
