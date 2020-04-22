// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Dash.UI;
using osu.Game.Rulesets.Objects.Drawables;
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

        [Resolved]
        private DashPlayfield playfield { get; set; }

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

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Hit:
                    var star = new DrawableNoteSheetCapStar
                    {
                        Origin = Anchor.Centre,
                        Anchor = LaneAnchor,
                        Size = Size,
                    };

                    var flash = new Circle
                    {
                        Origin = Anchor.Centre,
                        Anchor = LaneAnchor,
                        Size = Size,
                        Scale = new Vector2(0.5f),
                    };

                    star.UpdateColour(AccentColour.Value);
                    flash.Colour = AccentColour.Value.Lighten(0.5f);
                    flash.Alpha = 0.4f;

                    playfield.EffectContainer.AddRange(new Drawable[]
                    {
                        star, flash
                    });

                    const float animation_time = 200f;

                    star.ScaleTo(2f, animation_time);
                    star.FadeOutFromOne(animation_time);

                    flash.ScaleTo(4f, animation_time / 2)
                         .Then()
                         .ScaleTo(0.5f, animation_time / 2)
                         .FadeOut(animation_time / 2);

                    break;
            }
        }

        public void UpdateResult() => base.UpdateResult(true);

        public override bool OnPressed(DashAction action) => false; // Handled by the hold note

        public override void OnReleased(DashAction action)
        {
        }
    }
}
