// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Dash.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetHead : DrawableLanedHit<NoteSheetHead>
    {
        private readonly DrawableNoteSheet noteSheet;
        private const double rotation_time = 1000;

        private readonly Sprite sprite;

        public DrawableNoteSheetHead(DrawableNoteSheet noteSheet)
            : base(noteSheet.HitObject.Head)
        {
            this.noteSheet = noteSheet;

            Size = new Vector2(DashPlayfield.HIT_TARGET_SIZE * 2);
            Origin = Anchor.Centre;

            AddInternal(sprite = new Sprite
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                FillMode = FillMode.Fit,
                RelativeSizeAxes = Axes.Both
            });
        }

        public void UpdateResult() => base.UpdateResult(true);

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) =>
            Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            sprite.Texture = store.Get("star");
        }

        protected override void Update()
        {
            base.Update();

            sprite.Rotation = (float)(Time.Current % rotation_time / rotation_time) * 360f;
        }

        public override bool OnPressed(DashAction action) => false; // Handled by the hold note

        public override void OnReleased(DashAction action)
        {
        }
    }
}
