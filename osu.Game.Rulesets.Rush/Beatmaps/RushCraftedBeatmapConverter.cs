// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public class RushCraftedBeatmapConverter : BeatmapConverter<RushHitObject>
    {
        public RushCraftedBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => throw new System.NotImplementedException();

        protected override IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap) => throw new System.NotImplementedException();
    }
}
