// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public class HeartPiece : CompositeDrawable
    {
        public HeartPiece()
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                new SpriteIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Icon = FontAwesome.Solid.Heart,
                    Colour = Color4.HotPink,
                },
                new SpriteIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    FillMode = FillMode.Fit,
                    Icon = FontAwesome.Solid.Heart,
                    Size = new Vector2(0.8f),
                    Colour = Color4.Red
                }
            };
        }
    }
}
