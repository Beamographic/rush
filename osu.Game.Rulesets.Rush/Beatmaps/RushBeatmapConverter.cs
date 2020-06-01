// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
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
            Random random = new Random((int)original.StartTime);

            const float air_position_cutoff = 180f;
            const float ground_position_cutoff = 220f;
            const double etna_cutoff = 200d;
            const double repeat_cutoff = 100d;
            const double sawblade_cutoff = 0.9f;
            const double airsawblade_cutoff = 0.95f;

            var sampleLane = original.Samples.Any(s => s.Name == HitSampleInfo.HIT_CLAP || s.Name == HitSampleInfo.HIT_WHISTLE) ? LanedHitLane.Air : LanedHitLane.Ground;
            LanedHitLane? positionLane = null, sawbladeLane = null;
            HitObjectType hitObjectType = HitObjectType.Minion;
            bool bothLanes = false;

            if (original is IHasPosition hasPosition)
            {
                if (hasPosition.Y < air_position_cutoff)
                    positionLane = LanedHitLane.Air;
                else if (hasPosition.Y > ground_position_cutoff)
                    positionLane = LanedHitLane.Ground;
                else
                    bothLanes = true;
            }

            if (original is IHasEndTime hasEndTime)
            {
                // etna sliders don't convert well, so just make them regular minions
                if (hasEndTime.Duration <= etna_cutoff)
                    hitObjectType = HitObjectType.Minion;
                else if (original is IHasDistance)
                    hitObjectType = HitObjectType.NoteSheet;
                else
                    hitObjectType = HitObjectType.MiniBoss;
            }

            // temporary sawblade selection logic
            // 1) can only convert minions or dual orbs
            // 2) ground sawblades are more common than air sawblades
            // 3) air sawblades can only exist during kiai sections
            if (hitObjectType == HitObjectType.Minion)
            {
                var rnd = random.NextDouble();
                sawbladeLane = LanedHitLane.Ground;
                if (rnd >= sawblade_cutoff)
                    hitObjectType = HitObjectType.Sawblade;
                if (original.Kiai && rnd >= airsawblade_cutoff)
                    sawbladeLane = LanedHitLane.Air;
            }

            switch (hitObjectType)
            {
                case HitObjectType.Sawblade:
                    if (bothLanes)
                    {
                        yield return new Minion
                        {
                            Lane = (sawbladeLane ?? sampleLane).Opposite(),
                            Samples = original.Samples,
                            StartTime = original.StartTime
                        };
                    }

                    yield return new Sawblade
                    {
                        Lane = sawbladeLane ?? sampleLane,
                        StartTime = original.StartTime
                    };

                    break;

                case HitObjectType.Minion:
                    if (bothLanes)
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

                case HitObjectType.MiniBoss:
                    yield return new MiniBoss
                    {
                        Samples = original.Samples,
                        StartTime = original.StartTime,
                        EndTime = original.GetEndTime()
                    };

                    break;

                case HitObjectType.NoteSheet:
                    var sheetLane = positionLane ?? sampleLane;

                    if (bothLanes || sheetLane == LanedHitLane.Ground)
                    {
                        yield return new NoteSheet
                        {
                            Lane = LanedHitLane.Ground,
                            Samples = original.Samples,
                            StartTime = original.StartTime,
                            EndTime = original.GetEndTime()
                        };
                    }

                    if (bothLanes || sheetLane == LanedHitLane.Air)
                    {
                        yield return new NoteSheet
                        {
                            Lane = LanedHitLane.Air,
                            Samples = bothLanes ? new List<HitSampleInfo>() : original.Samples,
                            StartTime = original.StartTime,
                            EndTime = original.GetEndTime()
                        };
                    }

                    if (!bothLanes && original is IHasRepeats hasRepeats && hasRepeats.RepeatCount > 0)
                    {
                        var duration = original.GetEndTime() - original.StartTime;
                        var repeatDuration = duration / hasRepeats.SpanCount();
                        var skip = 1;

                        // Currently an issue where an odd number of repeats (span count) will skip
                        // the final minion if repeats are too short. Not sure what to do here since
                        // it doesn't make rhythmic sense to add an extra hit object.
                        // Examples:
                        //   *-*-*-*-* becomes *---*---* (good)
                        //   *-*-*-*   becomes *---*-- (looks bad) instead of *---*-* (rhythmically worse)
                        while (repeatDuration < repeat_cutoff)
                        {
                            repeatDuration *= 2;
                            skip *= 2;
                        }

                        var otherLane = sheetLane.Opposite();
                        var repeatCurrent = original.StartTime;
                        var index = -1;

                        foreach (var nodeSample in hasRepeats.NodeSamples)
                        {
                            index++;

                            if (index % skip != 0)
                                continue;

                            yield return new Minion
                            {
                                Lane = otherLane,
                                Samples = nodeSample,
                                StartTime = repeatCurrent
                            };

                            repeatCurrent += repeatDuration;
                        }
                    }

                    break;
            }
        }

        private enum HitObjectType
        {
            Minion,
            NoteSheet,
            MiniBoss,
            Sawblade
        }
    }
}
