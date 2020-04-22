// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetBody : DrawableDashHitObject<NoteSheetBody>
    {
        public Bindable<bool> HasBroken { get; } = new BindableBool();

        private readonly DrawableNoteSheet noteSheet;

        private const float border_size = 1f / 8f;

        private readonly Box backgroundBox;
        private readonly Triangles triangles;
        private readonly Box topBox;
        private readonly Box bottomBox;

        public DrawableNoteSheetBody(DrawableNoteSheet noteSheet)
            : base(noteSheet.HitObject.Body)
        {
            this.noteSheet = noteSheet;

            RelativeSizeAxes = Axes.Y;
            Height = 1f;

            AddRangeInternal(new Drawable[]
            {
                backgroundBox = new Box { RelativeSizeAxes = Axes.Both },
                triangles = new Triangles { RelativeSizeAxes = Axes.Both },
                topBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = border_size,
                    Origin = Anchor.TopCentre,
                    Anchor = Anchor.TopCentre,
                },
                bottomBox = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Height = border_size,
                    Origin = Anchor.BottomCentre,
                    Anchor = Anchor.BottomCentre,
                }
            });

            AccentColour.ValueChanged += _ => updateDrawables();
            HasBroken.ValueChanged += _ => updateDrawables();
        }

        private void updateDrawables()
        {
            var colour = HasBroken.Value ? Color4.Gray : AccentColour.Value;
            backgroundBox.Colour = colour.Darken(1f);
            topBox.Colour = bottomBox.Colour = colour;
            triangles.Colour = colour;
            triangles.Alpha = 0.8f;
        }

        protected override void Update()
        {
            base.Update();

            Width = noteSheet.DrawWidth;
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            Origin = Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreRight : Anchor.CentreLeft;
        }

        public void UpdateResult() => base.UpdateResult(true);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged || timeOffset < 0)
                return;

            ApplyResult(r => r.Type = HasBroken.Value ? HitResult.Miss : HitResult.Perfect);
        }

        public override bool OnPressed(DashAction action) => false; // Handled by the hold note

        public override void OnReleased(DashAction action)
        {
        }
    }
}
