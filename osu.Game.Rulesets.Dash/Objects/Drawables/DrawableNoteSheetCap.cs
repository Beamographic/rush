// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osuTK;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetCap<TObject> : DrawableLanedHit<TObject>
        where TObject : LanedHit
    {
        private readonly DrawableNoteSheetCapStar capStar;
        protected readonly DrawableNoteSheet NoteSheet;

        public DrawableNoteSheetCap(DrawableNoteSheet noteSheet, TObject hitObject)
            : base(hitObject)
        {
            NoteSheet = noteSheet;
            Size = new Vector2(DrawableNoteSheet.NOTE_SHEET_SIZE * 1.1f);
            Origin = Anchor.Centre;

            Content.Child = capStar = new DrawableNoteSheetCapStar
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both
            };
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            AccentColour.BindValueChanged(evt => capStar.UpdateColour(evt.NewValue), true);
        }

        public void UpdateResult() => base.UpdateResult(true);

        public override bool OnPressed(DashAction action) => false; // Handled by the hold note

        public override void OnReleased(DashAction action)
        {
        }
    }
}
