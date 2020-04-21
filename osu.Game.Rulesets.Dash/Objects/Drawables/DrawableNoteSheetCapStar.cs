// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Backgrounds;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetCapStar : CompositeDrawable
    {
        private const double rotation_time = 1000;

        private readonly SpriteIcon spriteIcon;
        private readonly Box backgroundBox;
        private readonly Triangles triangles;

        public void UpdateColour(Color4 colour)
        {
            backgroundBox.Colour = colour.Darken(0.5f);
            triangles.Colour = colour;
            triangles.Alpha = 0.8f;
            spriteIcon.Colour = colour.Lighten(0.5f);
        }

        public DrawableNoteSheetCapStar()
        {
            AddRangeInternal(new Drawable[]
            {
                spriteIcon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Scale = new Vector2(1.5f),
                    RelativeSizeAxes = Axes.Both,
                    Icon = FontAwesome.Solid.Star,
                },
                new CircularContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    BorderThickness = DrawableNoteSheet.NOTE_SHEET_SIZE * 0.1f,
                    BorderColour = Color4.White,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        backgroundBox = new Box { RelativeSizeAxes = Axes.Both },
                        triangles = new Triangles { RelativeSizeAxes = Axes.Both }
                    }
                }
            });
        }

        protected override void Update()
        {
            base.Update();

            spriteIcon.OriginPosition = new Vector2(DrawWidth * 0.5f, DrawHeight * 0.54f);
            spriteIcon.Rotation = (float)(Time.Current % rotation_time / rotation_time) * 360f;
        }
    }
}
