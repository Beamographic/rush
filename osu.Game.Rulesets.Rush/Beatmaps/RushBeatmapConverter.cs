// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Objects.Types;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public class RushBeatmapConverter : BeatmapConverter<RushHitObject>
    {
        public RushBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        // todo: Check for conversion types that should be supported (ie. Beatmap.HitObjects.Any(h => h is IHasXPosition))
        // https://github.com/ppy/osu/tree/master/osu.Game/Rulesets/Objects/Types
        public override bool CanConvert() => true;

        protected override IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            var sampleLane = original.Samples.Any(s => s.Name == HitSampleInfo.HIT_CLAP || s.Name == HitSampleInfo.HIT_WHISTLE) ? LanedHitLane.Air : LanedHitLane.Ground;
            LanedHitLane? positionLane = null;
            bool dualOrb = false;

            if (original is IHasPosition hasPosition)
            {
                if (hasPosition.Y < 180)
                    positionLane = LanedHitLane.Air;
                else if (hasPosition.Y > 220)
                    positionLane = LanedHitLane.Ground;
                else
                    dualOrb = true;
            }

            switch (original)
            {
                case IHasDistance hasDistance:
                    yield return new NoteSheet
                    {
                        Lane = positionLane ?? sampleLane,
                        Samples = original.Samples,
                        StartTime = original.StartTime,
                        EndTime = hasDistance.EndTime
                    };

                    break;

                case IHasEndTime hasEndTime:
                    yield return new MiniBoss
                    {
                        Samples = original.Samples,
                        StartTime = original.StartTime,
                        EndTime = hasEndTime.EndTime
                    };

                    break;

                default:
                    if (dualOrb)
                    {
                        yield return new DualOrb
                        {
                            Samples = original.Samples,
                            StartTime = original.StartTime,
                        };
                    }
                    else
                    {
                        yield return new Minion
                        {
                            Lane = positionLane ?? sampleLane,
                            Samples = original.Samples,
                            StartTime = original.StartTime,
                        };
                    }

                    break;
            }
        }
    }
}
