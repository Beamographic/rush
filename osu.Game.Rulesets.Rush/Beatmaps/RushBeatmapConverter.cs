// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    /// <summary>
    /// Wraps <see cref="RushCraftedBeatmapConverter"/> and <see cref="RushGeneratedBeatmapConverter"/> depending
    /// on whether the original beatmap contains a given tag. Once the lazer editor has been fleshed out enough
    /// to explicitly choose rulesets, this will be unnecessary.
    /// </summary>
    public class RushBeatmapConverter : IBeatmapConverter
    {
        private const string crafted_tag = "crafted";
        private readonly BeatmapConverter<RushHitObject> converter;

        public RushBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
        {
            bool crafted = !string.IsNullOrEmpty(beatmap.Metadata.Tags)
                           && beatmap.Metadata.Tags
                                     .Split(" ")
                                     .Any(tag => tag.Equals(crafted_tag, StringComparison.InvariantCultureIgnoreCase));

            converter = crafted ? (BeatmapConverter<RushHitObject>)new RushCraftedBeatmapConverter(beatmap, ruleset) : new RushGeneratedBeatmapConverter(beatmap, ruleset);
        }

        public bool CanConvert() => converter.CanConvert();

        public IBeatmap Convert() => converter.Convert();

        public IBeatmap Beatmap => converter.Beatmap;

        /// <summary>
        /// <see cref="BeatmapConverter{T}"/>'s implementation is not visible to us,
        /// so we can't defer to it.
        /// </summary>
        public event Action<HitObject, IEnumerable<HitObject>> ObjectConverted;
    }
}
