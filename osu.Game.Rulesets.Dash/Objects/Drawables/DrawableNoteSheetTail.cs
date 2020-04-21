// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetTail : DrawableNoteSheetCap<NoteSheetTail>
    {
        private const double release_window_lenience = 1.5;

        public DrawableNoteSheetTail(DrawableNoteSheet noteSheet)
            : base(noteSheet, noteSheet.HitObject.Tail)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            // Factor in the release lenience
            timeOffset /= release_window_lenience;

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);
            if (result == HitResult.None)
                return;

            ApplyResult(r =>
            {
                // If the head wasn't hit or the hold note was broken, cap the max score to Meh.
                if (result > HitResult.Meh && (!NoteSheet.Head.IsHit || NoteSheet.HasBroken))
                    result = HitResult.Meh;

                r.Type = result;
            });
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) =>
            Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
    }
}
