// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableNoteSheetTail : DrawableNoteSheetCap<NoteSheetTail>
    {
        private const double release_window_lenience = 3;

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
                {
                    ApplyResult(r => r.Type = HitResult.Miss);
                    HasBroken.Value = true;
                }

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);
            if (result == HitResult.None)
                return;

            ApplyResult(r => r.Type = HasBroken.Value ? HitResult.Miss : result);
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) =>
            Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
    }
}
