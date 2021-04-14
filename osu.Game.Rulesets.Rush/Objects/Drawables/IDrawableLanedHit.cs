// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public interface IDrawableLanedHit
    {
        Color4 LaneAccentColour { get; }
        Anchor LaneAnchor { get; }

        LanedHitLane Lane { get; set; }
    }
}
