// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetHead : DrawableNoteSheetCap<NoteSheetHead>
    {
        public DrawableNoteSheetHead(DrawableNoteSheet noteSheet)
            : base(noteSheet, noteSheet.HitObject.Head)
        {
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) =>
            Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
    }
}
