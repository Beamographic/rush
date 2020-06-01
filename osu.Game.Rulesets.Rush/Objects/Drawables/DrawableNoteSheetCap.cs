// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableNoteSheetCap<TObject> : DrawableLanedHit<TObject>
        where TObject : LanedHit
    {
        public Bindable<bool> HasBroken { get; } = new BindableBool();

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

        protected override void UpdateInitialTransforms()
        {
            Scale = Vector2.One;
            Alpha = 1f;
        }

        public void UpdateResult() => base.UpdateResult(true);

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
        }

        public override bool OnPressed(RushAction action) => false; // Handled by the hold note

        public override void OnReleased(RushAction action)
        {
        }
    }
}
