// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Rush.UI
{
    public class RushLanedPlayfield : ScrollingPlayfield
    {
        public LanedHitLane Lane { get; }

        public RushLanedPlayfield(LanedHitLane lane)
        {
            Lane = lane;
            Name = $"{Lane} Playfield";

            InternalChildren = new Drawable[]
            {
                HitObjectContainer
            };
        }
    }
}
