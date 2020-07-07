// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Replays;
using osu.Game.Scoring;
using osu.Game.Users;

namespace osu.Game.Rulesets.Rush.Mods
{
    public class RushModAutoplay : ModAutoplay<RushHitObject>
    {
        public override Score CreateReplayScore(IBeatmap beatmap) => new Score
        {
            ScoreInfo = new ScoreInfo
            {
                User = new User { Username = "Autoplay" },
            },
            Replay = new RushAutoGenerator(beatmap).Generate(),
        };
    }
}
