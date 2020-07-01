// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableNoteSheetCap<TObject> : DrawableLanedHit<TObject>
        where TObject : LanedHit
    {
        private readonly DrawableNoteSheetCapStar capStar;
        protected readonly DrawableNoteSheet NoteSheet;

        [Resolved]
        private RushPlayfield playfield { get; set; }

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
                RelativeSizeAxes = Axes.Both,
                AccentColour = { BindTarget = AccentColour },
            };
        }

        protected override void UpdateInitialTransforms()
        {
            Scale = Vector2.One;
            Alpha = 1f;
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            base.UpdateStateTransforms(state);

            if (state == ArmedState.Hit)
                Hide();
        }

        public bool TriggerResult() => UpdateResult(true);

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
        }

        public override bool OnPressed(RushAction action) => false; // Handled by the note sheet object itself.

        public override void OnReleased(RushAction action)
        {
        }
    }
}
