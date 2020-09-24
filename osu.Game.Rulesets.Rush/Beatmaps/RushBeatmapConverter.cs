// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    /// <summary>
    /// Wraps <see cref="RushCraftedBeatmapConverter"/> and <see cref="RushGeneratedBeatmapConverter"/> depending
    /// on whether the original beatmap contains a given tag. Once the lazer editor has been fleshed out enough
    /// to explicitly choose rulesets, this will be unnecessary.
    /// </summary>
    public class RushBeatmapConverter : IBeatmapConverter
    {
        public readonly IBeatmapConverter BackedConverter;
        public const string CRAFTED_TAG = "crafted";

        public RushBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
        {
            bool crafted = !string.IsNullOrEmpty(beatmap.Metadata.Tags)
                           && beatmap.Metadata.Tags
                                     .Split(" ")
                                     .Any(tag => tag.Equals(CRAFTED_TAG, StringComparison.InvariantCultureIgnoreCase));

            BackedConverter = crafted
                ? (IBeatmapConverter)new RushCraftedBeatmapConverter(beatmap, ruleset)
                : new RushGeneratedBeatmapConverter(beatmap, ruleset);
        }

        public event Action<HitObject, IEnumerable<HitObject>> ObjectConverted
        {
            add => BackedConverter.ObjectConverted += value;
            remove => BackedConverter.ObjectConverted -= value;
        }

        public bool CanConvert() => BackedConverter.CanConvert();
        public IBeatmap Convert(CancellationToken cancellationToken = default) => BackedConverter.Convert(cancellationToken);

        public IBeatmap Beatmap => BackedConverter.Beatmap;
    }
}
