// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Objects.Types;
using osuTK;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public class RushBeatmapConverter : BeatmapConverter<RushHitObject>
    {
        private const float half_height = 200f;

        private const double skip_probability = 0.1;
        private const double sawblade_probability = 0.1;
        private const double orb_probability = 0.1;
        private const double suggest_probability = 0.2;
        private const double kiai_multiplier = 3;

        private const double sawblade_same_lane_safety_time = 100;
        private const double sawblade_fall_safety_near_time = 80;
        private const double sawblade_fall_safety_far_time = 600;
        private const double min_sawblade_time = 500;
        private const double min_heart_time = 30000;
        private const double min_orb_time = 500;

        private double nextHeartTime;
        private double nextDualOrbTime;
        private double nextSawbladeTime;

        private double lastSawbladeTime;
        private LanedHitLane? lastSawbladeLane;
        private LanedHitLane? previousLane;
        private Vector2? previousSourcePosition;
        private double previousSourceTime;

        public RushBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        protected override Beatmap<RushHitObject> CreateBeatmap() => new RushBeatmap();

        protected override Beatmap<RushHitObject> ConvertBeatmap(IBeatmap original)
        {
            var firstObject = original.HitObjects.FirstOrDefault()?.StartTime ?? 0;
            nextHeartTime = firstObject + min_heart_time;
            nextDualOrbTime = firstObject + min_orb_time;
            nextSawbladeTime = firstObject + min_sawblade_time;
            lastSawbladeLane = null;
            previousLane = null;
            previousSourcePosition = null;
            previousSourceTime = 0;

            return base.ConvertBeatmap(original);
        }

        // todo: Check for conversion types that should be supported (ie. Beatmap.HitObjects.Any(h => h is IHasXPosition))
        // https://github.com/ppy/osu/tree/master/osu.Game/Rulesets/Objects/Types
        public override bool CanConvert() => true;

        protected override IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            // TODO: for now we'll skip notesheets as they'll require some funky logic

            // if it's definitely a spinner, return a miniboss
            if (original is IHasDuration && !(original is IHasDistance))
            {
                yield return new MiniBoss { Samples = original.Samples, StartTime = original.StartTime, EndTime = original.GetEndTime() };

                previousLane = null;
                previousSourceTime = original.GetEndTime();
                previousSourcePosition = null;
                yield break;
            }

            // otherwise do some flag magic
            Random random = new Random((int)original.StartTime);

            HitObjectFlags flags = flagsForHitObject(original, beatmap);
            LanedHitLane? lane = null;
            var kiaiMultiplier = original.Kiai ? kiai_multiplier : 1;

            // try to get a lane from the force flags
            if (flags.HasFlag(HitObjectFlags.ForceSameLane) || flags.HasFlag(HitObjectFlags.SuggestSameLane) && random.NextDouble() < suggest_probability)
                lane = previousLane;
            else if (flags.HasFlag(HitObjectFlags.ForceNotSameLane) || flags.HasFlag(HitObjectFlags.SuggestNotSameLane) && random.NextDouble() < suggest_probability)
                lane = previousLane?.Opposite();

            // if it's low probability, potentially skip this object
            if (flags.HasFlag(HitObjectFlags.LowProbability) && random.NextDouble() < skip_probability)
            {
                previousLane = lane ?? previousLane;
                previousSourceTime = original.GetEndTime();
                previousSourcePosition = (original as IHasPosition)?.Position;
                yield break;
            }

            // if we don't have a lane and not too close to a sawblade, allow adding a double hit
            if (lane == null
                && original.StartTime - lastSawbladeTime >= sawblade_same_lane_safety_time
                && flags.HasFlag(HitObjectFlags.AllowDoubleHit)
                && original.StartTime >= nextDualOrbTime
                && random.NextDouble() < orb_probability)
            {
                nextDualOrbTime = original.StartTime + min_orb_time;
                yield return new DualOrb { StartTime = original.StartTime, Samples = original.Samples };

                previousLane = null;
                previousSourceTime = original.GetEndTime();
                previousSourcePosition = (original as IHasPosition)?.Position;
                yield break;
            }

            // if we still haven't selected a lane at this point, do it based on position, defaulting to ground
            var finalLane = lane ?? laneForHitObject(original) ?? LanedHitLane.Ground;

            var timeSinceLastSawblade = original.StartTime - lastSawbladeTime;
            var tooCloseToLastSawblade = finalLane == lastSawbladeLane && timeSinceLastSawblade < sawblade_same_lane_safety_time;

            bool sawbladeAdded = false;

            // if we are allowed to add or replace a sawblade, potentially do it
            if ((flags & HitObjectFlags.AllowSawbladeAddOrReplace) != 0 && original.StartTime >= nextSawbladeTime && kiaiMultiplier * random.NextDouble() < sawblade_probability)
            {
                // the sawblade will always appear in the opposite lane to where the player is expected to hit
                var sawbladeLane = finalLane.Opposite();

                // if the sawblade time is less than twice the minimum gap, it must be in the opposite lane to its previous one
                if (original.StartTime - nextSawbladeTime < 2 * min_sawblade_time)
                    sawbladeLane = lastSawbladeLane?.Opposite() ?? LanedHitLane.Ground;

                // if the new sawblade is too close to the previous hit in the same lane, skip it
                var tooCloseToSameLane = previousLane == null || previousLane == sawbladeLane && original.StartTime - previousSourceTime < sawblade_same_lane_safety_time;

                // if a ground sawblade is too far from the previous hit in the air lane, skip it (as the player may not have time to jump upon landing)
                var canFallOntoSawblade = previousLane == LanedHitLane.Air && sawbladeLane == LanedHitLane.Ground && original.StartTime - previousSourceTime > sawblade_fall_safety_near_time
                                          && original.StartTime - previousSourceTime < sawblade_fall_safety_far_time;

                // air sawblades may only appear in a kiai section, and not too close to a hit in the same lane (or laneless)
                // also need to account for a gap where the player may fall onto the blade
                if (!tooCloseToSameLane && !canFallOntoSawblade && (sawbladeLane == LanedHitLane.Ground || original.Kiai))
                {
                    sawbladeAdded = true;
                    lastSawbladeLane = sawbladeLane;
                    lastSawbladeTime = original.StartTime;
                    nextSawbladeTime = original.StartTime + min_sawblade_time;

                    // add a sawblade
                    yield return new Sawblade
                    {
                        StartTime = original.StartTime,
                        Lane = sawbladeLane
                    };

                    // absolutely need to make sure that we never try to add a hit to the same lane as the sawblade that was just added
                    finalLane = sawbladeLane.Opposite();
                }
            }

            // we can add a regular hit if:
            //   we didn't add a sawblade, or
            //   we added a sawblade and are allowed to replace the hit entirely, or
            //   we added a sawblade that was in the opposite lane
            if (!tooCloseToLastSawblade && (!sawbladeAdded || !flags.HasFlag(HitObjectFlags.AllowSawbladeReplace)))
            {
                // if it's time to add a heart, we must do so
                if (original.StartTime >= nextHeartTime)
                {
                    nextHeartTime += min_heart_time;

                    yield return new Heart
                    {
                        StartTime = original.StartTime,
                        Samples = original.Samples,
                        Lane = finalLane,
                    };
                }
                else
                {
                    yield return new Minion
                    {
                        StartTime = original.StartTime,
                        Samples = original.Samples,
                        Lane = finalLane,
                    };
                }
            }

            // update "previous" info for the next hitobject
            previousLane = finalLane;
            previousSourceTime = original.GetEndTime();
            previousSourcePosition = (original as IHasPosition)?.Position;
        }

        private LanedHitLane? laneForHitObject(HitObject hitObject) =>
            hitObject is IHasYPosition hasYPosition ? (LanedHitLane?)(hasYPosition.Y < half_height ? LanedHitLane.Air : LanedHitLane.Ground) : null;

        private HitObjectFlags flagsForHitObject(HitObject hitObject, IBeatmap beatmap)
        {
            HitObjectFlags flags = HitObjectFlags.None;

            TimingControlPoint timingPoint = beatmap.ControlPointInfo.TimingPointAt(hitObject.StartTime);
            EffectControlPoint effectPoint = beatmap.ControlPointInfo.EffectPointAt(hitObject.StartTime);

            var positionData = hitObject as IHasPosition;
            var newCombo = (hitObject as IHasCombo)?.NewCombo ?? false;

            float positionSeparation = ((positionData?.Position ?? Vector2.Zero) - (previousSourcePosition ?? Vector2.Zero)).Length;
            double timeSeparation = hitObject.StartTime - previousSourceTime;

            if (timeSeparation <= 80)
            {
                // more than 187 bpm
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.ForceSameLane;
                flags |= HitObjectFlags.AllowSawbladeAdd;
            }
            else if (timeSeparation <= 105)
            {
                // more than 140 bpm
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.SuggestNotSameLane;
                flags |= HitObjectFlags.LowProbability | HitObjectFlags.AllowSawbladeAdd;
            }
            else if (timeSeparation <= 125)
            {
                // more than 120 bpm
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.SuggestNotSameLane;
                flags |= HitObjectFlags.AllowSawbladeAdd;
            }
            else if (timeSeparation <= 135 && positionSeparation < 20)
            {
                // more than 111 bpm stream
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.ForceSameLane;
                flags |= HitObjectFlags.AllowSawbladeAdd;
            }
            else
            {
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.ForceSameLane;
                flags |= HitObjectFlags.LowProbability | HitObjectFlags.AllowSawbladeReplace | HitObjectFlags.AllowDoubleHit;
            }

            // new combo should never be low probability
            if (newCombo) flags &= ~HitObjectFlags.LowProbability;

            return flags;
        }

        [Flags]
        private enum HitObjectFlags
        {
            None = 0,

            /// <summary>
            /// Ensures that the next object will be in the same lane.
            /// </summary>
            ForceSameLane = 1 << 0,

            /// <summary>
            /// Ensures that the next object will not be in the same lane.
            /// </summary>
            ForceNotSameLane = 1 << 1,

            /// <summary>
            /// Indicates that the next object should consider staying in the same lane.
            /// </summary>
            SuggestSameLane = 1 << 2,

            /// <summary>
            /// Indicates that the next object should consider changing lanes.
            /// </summary>
            SuggestNotSameLane = 1 << 3,

            /// <summary>
            /// Indicates that the next object will be less likely to appear.
            /// </summary>
            LowProbability = 1 << 4,

            /// <summary>
            /// Indicates that the next object may be replaced with double hit.
            /// </summary>
            AllowDoubleHit = 1 << 5,

            /// <summary>
            /// Indicates that the next object may be completely replaced with a sawblade in the opposite lane.
            /// </summary>
            AllowSawbladeReplace = 1 << 6,

            /// <summary>
            /// Indicates that the next object may additionally add a sawblade to the opposite lane.
            /// </summary>
            AllowSawbladeAdd = 1 << 7,

            AllowSawbladeAddOrReplace = AllowSawbladeAdd | AllowSawbladeReplace,
        }
    }
}
