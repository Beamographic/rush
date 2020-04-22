// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetCap<TObject> : DrawableLanedHit<TObject>
        where TObject : LanedHit
    {
        public Bindable<bool> HasBroken { get; } = new BindableBool();

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

            AccentColour.ValueChanged += _ => updateDrawables();
            HasBroken.ValueChanged += _ => updateDrawables();
        }

        private void updateDrawables()
        {
            var colour = HasBroken.Value ? Color4.Gray : AccentColour.Value;
            capStar.UpdateColour(colour);
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
