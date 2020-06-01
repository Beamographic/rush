// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableNoteSheetTail : DrawableNoteSheetCap<NoteSheetTail>
    {
        internal const double RELEASE_WINDOW_LENIENCE = 3;

        public DrawableNoteSheetTail(DrawableNoteSheet noteSheet)
            : base(noteSheet, noteSheet.HitObject.Tail)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            // Factor in the release lenience
            timeOffset /= RELEASE_WINDOW_LENIENCE;

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

        // FIXME: should logically be TrailingAnchor, not sure why it renders incorrectly
        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) => Anchor = LeadingAnchor;
    }
}
