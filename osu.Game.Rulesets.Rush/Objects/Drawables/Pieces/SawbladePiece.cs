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
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public class SawbladePiece : CompositeDrawable
    {
        private const double rotation_time = 1000;

        private readonly SpriteIcon outerSawIcon;
        private readonly SpriteIcon innerSawIcon;
        private readonly Box backgroundBox;
        private readonly Triangles triangles;

        public readonly Bindable<Color4> AccentColour = new Bindable<Color4>();

        public SawbladePiece()
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new Drawable[]
            {
                outerSawIcon = new SpriteIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Icon = FontAwesome.Solid.Sun,
                    Colour = Color4.White,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(1.1f)
                },
                innerSawIcon = new SpriteIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Icon = FontAwesome.Solid.Sun,
                    RelativeSizeAxes = Axes.Both,
                },
                new CircularContainer
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    BorderThickness = DrawableStarSheet.STAR_SHEET_SIZE * 0.1f,
                    BorderColour = Color4.White,
                    Masking = true,
                    Size = new Vector2(0.75f),
                    Children = new Drawable[]
                    {
                        backgroundBox = new Box { RelativeSizeAxes = Axes.Both },
                        triangles = new Triangles { RelativeSizeAxes = Axes.Both }
                    }
                }
            };
        }

        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] DrawableHitObject drawableHitObject)
        {
            if (drawableHitObject != null)
                AccentColour.BindTo(drawableHitObject.AccentColour);

            AccentColour.BindValueChanged(evt =>
            {
                backgroundBox.Colour = evt.NewValue.Darken(0.5f);
                triangles.Colour = evt.NewValue;
                triangles.Alpha = 0.8f;
                innerSawIcon.Colour = evt.NewValue.Lighten(0.5f);
            }, true);
        }

        protected override void Update()
        {
            base.Update();

            innerSawIcon.Rotation = outerSawIcon.Rotation = (float)(Time.Current % rotation_time / rotation_time) * 360f;
        }
    }
}
