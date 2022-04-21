// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Replays;

namespace osu.Game.Rulesets.Rush.Mods
{
    public class RushModCinema : ModCinema<RushHitObject>
    {
        public override ModReplayData CreateReplayData(IBeatmap beatmap, IReadOnlyList<Mod> mods)
        {
            return new ModReplayData(new RushAutoGenerator(beatmap).Generate(), new ModCreatedUser { Username = "Autoplay" });
        }
    }
}
