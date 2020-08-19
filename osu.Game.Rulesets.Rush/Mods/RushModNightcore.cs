// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Mods
{
    public class RushModNightcore : ModNightcore<RushHitObject>
    {
        public override double ScoreMultiplier => 1.12;
    }
}
