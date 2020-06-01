// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableNoteSheetHead : DrawableNoteSheetCap<NoteSheetHead>
    {
        public DrawableNoteSheetHead(DrawableNoteSheet noteSheet)
            : base(noteSheet, noteSheet.HitObject.Head)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged)
                return;

            if (userTriggered)
            {
                var result = HitObject.HitWindows.ResultFor(timeOffset);
                if (result == HitResult.None)
                    return;

                ApplyResult(r => r.Type = result);

                if (result == HitResult.Miss)
                    HasBroken.Value = true;
            }
            else if (!HitObject.HitWindows.CanBeHit(timeOffset))
            {
                ApplyResult(r => r.Type = HitResult.Miss);
                HasBroken.Value = true;
            }
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) => Anchor = LeadingAnchor;
    }
}
