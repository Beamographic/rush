// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public class StarSheetBodyPiece : CompositeDrawable
    {
        private const float border_size = 1f / 8f;

        private readonly Box backgroundBox, topBox, bottomBox;
        private readonly Triangles triangles;

        public readonly Bindable<Color4> AccentColour = new Bindable<Color4>();

        public StarSheetBodyPiece()
        {
            RelativeSizeAxes = Axes.Y;

            InternalChildren = new Drawable[]
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
            };
        }

        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] DrawableHitObject drawableHitObject)
        {
            if (drawableHitObject != null)
                AccentColour.BindTo(drawableHitObject.AccentColour);

            AccentColour.BindValueChanged(c =>
            {
                backgroundBox.Colour = c.NewValue.Darken(1f);
                topBox.Colour = bottomBox.Colour = c.NewValue;
                triangles.Colour = c.NewValue;
                triangles.Alpha = 0.8f;
            }, true);
        }
    }
}
