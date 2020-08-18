// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public class DualHitJoinPiece : Box
    {
        public DualHitJoinPiece()
        {
            Anchor = Anchor.Centre;
            Origin = Anchor.Centre;
            RelativeSizeAxes = Axes.Y;
            Size = new Vector2(10f, 1f);
            Colour = ColourInfo.GradientVertical(DrawableRushHitObject.AIR_ACCENT_COLOUR, DrawableRushHitObject.GROUND_ACCENT_COLOUR);
        }
    }
}
