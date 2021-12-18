// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Online.API.Requests.Responses;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Rush.Replays;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Rush.Mods
{
    public class RushModAutoplay : ModAutoplay
    {
        public override Score CreateReplayScore(IBeatmap beatmap, IReadOnlyList<Mod> mods) => new Score
        {
            ScoreInfo = new ScoreInfo { User = new APIUser { Username = "Autoplay" } },
            Replay = new RushAutoGenerator(beatmap).Generate()
        };
    }
}
