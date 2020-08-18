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
using osu.Game.Rulesets.Rush.UI;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public class DualHitPartPiece : CompositeDrawable
    {
        private const double rotation_time = 1000;

        private readonly SpriteIcon gearSpriteIcon;
        private readonly Box background;
        private readonly Triangles triangles;

        public readonly Bindable<Color4> AccentColour = new Bindable<Color4>();

        public DualHitPartPiece()
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE);

            AddRangeInternal(new Drawable[]
            {
                gearSpriteIcon = new SpriteIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Icon = FontAwesome.Solid.Cog
                },
                new CircularContainer
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Scale = new Vector2(0.7f),
                    BorderThickness = RushPlayfield.HIT_TARGET_SIZE * 0.1f,
                    BorderColour = Color4.White,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        background = new Box { RelativeSizeAxes = Axes.Both },
                        triangles = new Triangles { RelativeSizeAxes = Axes.Both }
                    }
                },
            });
        }

        [BackgroundDependencyLoader]
        private void load([CanBeNull] DrawableHitObject drawableHitObject)
        {
            if (drawableHitObject != null)
                AccentColour.BindTo(drawableHitObject.AccentColour);

            AccentColour.BindValueChanged(evt =>
            {
                background.Colour = evt.NewValue.Darken(0.5f);
                triangles.Colour = evt.NewValue;
                triangles.Alpha = 0.8f;
                gearSpriteIcon.Colour = evt.NewValue.Lighten(0.5f);
            });
        }

        protected override void Update()
        {
            base.Update();

            gearSpriteIcon.Rotation = (float)(Time.Current % rotation_time / rotation_time) * 360f;
        }
    }
}
