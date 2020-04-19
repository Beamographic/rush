// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetHead : DrawableLanedHit<NoteSheetHead>
    {
        private readonly DrawableNoteSheet noteSheet;

        private readonly Sprite sprite;

        public DrawableNoteSheetHead(DrawableNoteSheet noteSheet)
            : base(noteSheet.HitObject.Head)
        {
            this.noteSheet = noteSheet;

            AddInternal(sprite = new Sprite
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Scale = new Vector2(0.5f)
            });
        }

        public void UpdateResult() => base.UpdateResult(true);

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) =>
            Origin = Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            sprite.Texture = store.Get("star");
        }

        public override bool OnPressed(DashAction action) => false; // Handled by the hold note

        public override void OnReleased(DashAction action)
        {
        }
    }
}
