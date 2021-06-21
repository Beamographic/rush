// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Rush.UI.Ground
{
    /// <summary>
    /// Represents a component displaying the ground the player will be standing on.
    /// </summary>
    public class GroundDisplay : CompositeDrawable
    {
        public GroundDisplay()
        {
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.TopCentre;
            RelativeSizeAxes = Axes.Both;
            Padding = new MarginPadding { Top = 50f };

            InternalChildren = new Drawable[]
            {
                new DefaultGround(),
            };
        }
    }
}
