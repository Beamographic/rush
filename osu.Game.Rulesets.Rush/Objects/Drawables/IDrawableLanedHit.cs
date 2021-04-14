// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public interface IDrawableLanedHit
    {
        LanedHitLane Lane { get; set; }
        Anchor LaneAnchor { get; }
        Color4 LaneAccentColour { get; }
    }
}
