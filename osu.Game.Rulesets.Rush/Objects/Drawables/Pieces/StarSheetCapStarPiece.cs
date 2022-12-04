// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public partial class StarSheetCapStarPiece : CompositeDrawable, IHasAccentColour
    {
        private const double rotation_time = 1000;

        private readonly SpriteIcon starIcon;
        private readonly Triangles triangles;
        private readonly Box backgroundBox;

        public readonly Bindable<Color4> AccentColour = new Bindable<Color4>();

        Color4 IHasAccentColour.AccentColour
        {
            get => AccentColour.Value;
            set => AccentColour.Value = value;
        }

        public StarSheetCapStarPiece()
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            Size = new Vector2(DrawableStarSheet.NOTE_SHEET_SIZE);

            AddRangeInternal(new Drawable[]
            {
                starIcon = new SpriteIcon
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
                    BorderThickness = DrawableStarSheet.NOTE_SHEET_SIZE * 0.1f,
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

        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] DrawableHitObject drawableHitObject)
        {
            if (drawableHitObject != null)
                AccentColour.BindTo(drawableHitObject.AccentColour);

            AccentColour.BindValueChanged(c =>
            {
                starIcon.Colour = c.NewValue.Lighten(0.5f);
                backgroundBox.Colour = c.NewValue.Darken(0.5f);
                triangles.Colour = c.NewValue;
                triangles.Alpha = 0.8f;
            }, true);
        }

        protected override void Update()
        {
            base.Update();

            starIcon.OriginPosition = new Vector2(DrawWidth * 0.5f, DrawHeight * 0.54f);
            starIcon.Rotation = (float)(Time.Current % rotation_time / rotation_time) * 360f;
        }
    }
}
