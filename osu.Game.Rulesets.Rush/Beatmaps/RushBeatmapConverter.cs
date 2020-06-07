// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
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
        private const double orb_probability = 0.2;
        private const double suggest_probability = 0.1;
        private const double notesheet_start_probability = 0.5;
        private const double notesheet_end_probability = 0.2;
        private const double notesheet_dual_probability = 0.1;
        private const double kiai_multiplier = 4;

        private const double sawblade_same_lane_safety_time = 100;
        private const double sawblade_fall_safety_near_time = 80;
        private const double sawblade_fall_safety_far_time = 600;
        private const double min_sawblade_time = 500;
        private const double min_heart_time = 30000;
        private const double min_orb_time = 500;
        private const double max_sheet_length = 2000;
        private const double min_sheet_length = 120;
        private const double notesheet_snap_time = 40;

        private double nextHeartTime;
        private double nextDualOrbTime;
        private double nextSawbladeTime;

        private double lastSawbladeTime;
        private LanedHitLane? lastSawbladeLane;
        private LanedHitLane? previousLane;
        private Vector2? previousSourcePosition;
        private double previousSourceTime;
        private HitObjectFlags previousFlags;

        private readonly Dictionary<LanedHitLane, NoteSheet> currentNoteSheets = new Dictionary<LanedHitLane, NoteSheet>();
        // private double? airNoteSheetStart;
        // private double? groundNoteSheetStart;

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
            currentNoteSheets.Clear();

            return base.ConvertBeatmap(original);
        }

        // todo: Check for conversion types that should be supported (ie. Beatmap.HitObjects.Any(h => h is IHasXPosition))
        // https://github.com/ppy/osu/tree/master/osu.Game/Rulesets/Objects/Types
        public override bool CanConvert() => true;

        protected override IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            void updatePrevious(LanedHitLane? newLane, HitObjectFlags newFlags)
            {
                previousLane = newLane;
                previousSourceTime = original.GetEndTime();
                previousSourcePosition = (original as IHasPosition)?.Position;
                previousFlags = newFlags;
            }

            // if it's definitely a spinner, return a miniboss
            if (original is IHasDuration && !(original is IHasDistance))
            {
                yield return createMiniBoss(original);

                updatePrevious(null, HitObjectFlags.None);
                yield break;
            }

            // otherwise do some flag magic
            Random random = new Random((int)original.StartTime);

            HitObjectFlags flags = flagsForHitObject(original, beatmap);

            // if no flags, completely skip this object
            if (flags == HitObjectFlags.None)
            {
                updatePrevious(previousLane, HitObjectFlags.None);
                yield break;
            }

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
                updatePrevious(lane ?? previousLane, flags);
                yield break;
            }

            // get the lane from the object
            lane ??= laneForHitObject(original);

            // if we should end a sheet, try to
            if (currentNoteSheets.Count > 0 && (flags.HasFlag(HitObjectFlags.ForceEndNotesheet) || flags.HasFlag(HitObjectFlags.SuggestEndNotesheet) && random.NextDouble() < notesheet_end_probability))
            {
                // randomly pick a lane from what's available
                var endLane = currentNoteSheets.Count == 1 ? currentNoteSheets.Single().Key : random.NextDouble() < 0.5 ? LanedHitLane.Ground : LanedHitLane.Air;

                // try to snap the endpoint to the current note
                if (original is IHasDistance && Math.Abs(original.StartTime - currentNoteSheets[endLane].EndTime) <= notesheet_snap_time)
                {
                    currentNoteSheets[endLane].EndTime = original.StartTime;
                    lane = endLane.Opposite();
                }

                currentNoteSheets.Remove(endLane);
            }

            // if we should start a notesheet...
            if (flags.HasFlag(HitObjectFlags.ForceStartNotesheet) || flags.HasFlag(HitObjectFlags.SuggestStartNotesheet) && random.NextDouble() < notesheet_start_probability)
            {
                LanedHitLane? extraLane = null;
                LanedHitLane sheetLane;
                NoteSheet sheet = null;
                bool addToFirst = true;

                // if no sheets active
                if (currentNoteSheets.Count == 0)
                {
                    // use the suggested lane or randomly select one
                    sheetLane = lane ?? (random.NextDouble() < 0.5 ? LanedHitLane.Ground : LanedHitLane.Air);

                    // create a sheet
                    currentNoteSheets[sheetLane] = sheet = createNoteSheet(original, sheetLane);
                    extraLane = sheetLane.Opposite();
                }
                // if there is one sheet active
                else if (currentNoteSheets.Count == 1)
                {
                    // get the free lane
                    sheetLane = currentNoteSheets.Single().Key.Opposite();

                    // end the existing sheet
                    currentNoteSheets[sheetLane.Opposite()].EndTime = original.StartTime;
                    currentNoteSheets.Remove(sheetLane.Opposite());

                    // create a new sheet
                    currentNoteSheets[sheetLane] = sheet = createNoteSheet(original, sheetLane);
                    extraLane = sheetLane.Opposite();
                    addToFirst = false;
                }
                // if two sheets active... just end one
                else
                {
                    // use the suggested lane or randomly select one
                    sheetLane = lane ?? (random.NextDouble() < 0.5 ? LanedHitLane.Ground : LanedHitLane.Air);

                    // end it
                    currentNoteSheets[sheetLane].EndTime = original.StartTime;
                    currentNoteSheets.Remove(sheetLane.Opposite());
                }

                // FIXME: surely this is bad, altering the hit object after it's been returned???
                if (sheet != null)
                    yield return sheet;

                // if still two sheets active for some reason, we're done
                if (currentNoteSheets.Count == 2)
                {
                    updatePrevious(sheetLane, flags);
                    yield break;
                }

                // add extra objects to the lane without a sheet
                if (original is IHasRepeats hasRepeats && extraLane != null)
                {

                }

                updatePrevious(sheetLane, flags);
                yield break;
            }

            // if either of the sheets are too long, end them where they are
            if (currentNoteSheets.ContainsKey(LanedHitLane.Air) && currentNoteSheets[LanedHitLane.Air].Duration >= max_sheet_length)
                currentNoteSheets.Remove(LanedHitLane.Air);

            if (currentNoteSheets.ContainsKey(LanedHitLane.Ground) && currentNoteSheets[LanedHitLane.Ground].Duration >= max_sheet_length)
                currentNoteSheets.Remove(LanedHitLane.Ground);

            // if not too close to a sawblade, allow adding a double hit
            if (original.StartTime - lastSawbladeTime >= sawblade_same_lane_safety_time
                && flags.HasFlag(HitObjectFlags.AllowDoubleHit)
                && original.StartTime >= nextDualOrbTime
                && random.NextDouble() < orb_probability)
            {
                nextDualOrbTime = original.StartTime + min_orb_time;
                yield return createDualOrb(original);

                updatePrevious(null, flags);
                yield break;
            }

            // if we still haven't selected a lane at this point, pick a random one
            var finalLane = lane ?? (random.NextDouble() < 0.5 ? LanedHitLane.Ground : LanedHitLane.Air);

            // check if a lane is blocked by a notesheet
            LanedHitLane? blockedLane = currentNoteSheets.ContainsKey(LanedHitLane.Air)
                ? LanedHitLane.Air
                : currentNoteSheets.ContainsKey(LanedHitLane.Ground)
                    ? LanedHitLane.Ground
                    : (LanedHitLane?)null;

            if (blockedLane != null && finalLane == blockedLane)
                finalLane = blockedLane.Value.Opposite();

            var timeSinceLastSawblade = original.StartTime - lastSawbladeTime;
            var tooCloseToLastSawblade = lane == lastSawbladeLane && timeSinceLastSawblade < sawblade_same_lane_safety_time;

            bool sawbladeAdded = false;

            // if we are allowed to add or replace a sawblade, potentially do it
            if ((flags & HitObjectFlags.AllowSawbladeAddOrReplace) != 0
                && original.StartTime >= nextSawbladeTime
                && kiaiMultiplier * random.NextDouble() < sawblade_probability)
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
                if (sawbladeLane != blockedLane
                    && !tooCloseToSameLane
                    && !canFallOntoSawblade
                    && (sawbladeLane == LanedHitLane.Ground || original.Kiai))
                {
                    sawbladeAdded = true;
                    lastSawbladeLane = sawbladeLane;
                    lastSawbladeTime = original.StartTime;
                    nextSawbladeTime = original.StartTime + min_sawblade_time;

                    // add a sawblade
                    yield return createSawblade(original, sawbladeLane);

                    // absolutely need to make sure that we never try to add a hit to the same lane as the sawblade that was just added
                    finalLane = sawbladeLane.Opposite();
                }
            }

            // we can add a regular hit if:
            //   we didn't add a sawblade, or
            //   we added a sawblade and are allowed to replace the hit entirely, or
            //   we added a sawblade that was in the opposite lane
            if (finalLane != blockedLane && !tooCloseToLastSawblade && (!sawbladeAdded || !flags.HasFlag(HitObjectFlags.AllowSawbladeReplace)))
                yield return createNormalHit(original, finalLane);

            updatePrevious(finalLane, flags);
        }

        private LanedHit createNormalHit(HitObject original, LanedHitLane lane, double? time = null)
        {
            time ??= original.StartTime;

            // if it's time to add a heart, we must do so
            if (time >= nextHeartTime)
            {
                nextHeartTime += min_heart_time;

                return new Heart
                {
                    StartTime = time.Value,
                    Samples = original.Samples,
                    Lane = lane,
                };
            }

            return new Minion
            {
                StartTime = time.Value,
                Samples = original.Samples,
                Lane = lane,
            };
        }

        private MiniBoss createMiniBoss(HitObject original) =>
            new MiniBoss
            {
                StartTime = original.StartTime,
                EndTime = original.GetEndTime(),
                Samples = original.Samples
            };

        private NoteSheet createNoteSheet(HitObject original, LanedHitLane lane) =>
            new NoteSheet
            {
                StartTime = original.StartTime,
                EndTime = original.GetEndTime(),
                Samples = original.Samples,
                Lane = lane
            };

        private DualOrb createDualOrb(HitObject original) =>
            new DualOrb
            {
                StartTime = original.StartTime,
                Samples = original.Samples
            };

        private Sawblade createSawblade(HitObject original, LanedHitLane lane) =>
            new Sawblade
            {
                StartTime = original.StartTime,
                Lane = lane
            };

        private LanedHitLane? laneForHitObject(HitObject hitObject) =>
            hitObject is IHasYPosition hasYPosition ? (LanedHitLane?)(hasYPosition.Y < half_height ? LanedHitLane.Air : LanedHitLane.Ground) : null;

        private HitObjectFlags flagsForHitObject(HitObject hitObject, IBeatmap beatmap)
        {
            HitObjectFlags flags = HitObjectFlags.None;

            // sliders should force a notesheet to start or end
            if (hitObject is IHasDuration hasDuration && hitObject is IHasDistance && hasDuration.Duration >= min_sheet_length)
            {
                // if (currentNoteSheets.Count == 2)
                //     flags |= HitObjectFlags.ForceStartNotesheet | HitObjectFlags.ForceEndNotesheet;
                // else
                flags |= HitObjectFlags.ForceStartNotesheet | HitObjectFlags.ForceEndNotesheet;
            }

            // TimingControlPoint timingPoint = beatmap.ControlPointInfo.TimingPointAt(hitObject.StartTime);
            // EffectControlPoint effectPoint = beatmap.ControlPointInfo.EffectPointAt(hitObject.StartTime);

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
                // flags |= HitObjectFlags.LowProbability;
                flags |= HitObjectFlags.AllowSawbladeAdd;
                flags |= HitObjectFlags.ForceEndNotesheet;
            }
            else if (timeSeparation <= 125)
            {
                // more than 120 bpm
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.SuggestNotSameLane;
                flags |= HitObjectFlags.AllowSawbladeAdd;
                flags |= HitObjectFlags.ForceEndNotesheet;
            }
            else if (timeSeparation <= 135 && positionSeparation < 20)
            {
                // more than 111 bpm stream
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.ForceSameLane;
                flags |= HitObjectFlags.AllowSawbladeAdd;
                flags |= HitObjectFlags.ForceEndNotesheet;
            }
            else
            {
                flags |= newCombo ? HitObjectFlags.ForceNotSameLane : HitObjectFlags.ForceSameLane;
                // flags |= HitObjectFlags.LowProbability;
                flags |= HitObjectFlags.AllowDoubleHit;
                flags |= HitObjectFlags.AllowSawbladeAdd;
                flags |= HitObjectFlags.ForceEndNotesheet;
                // flags |= HitObjectFlags.AllowSawbladeReplace; FIXME: for now, always add rather than replace
            }

            // new combo should never be low probability
            if (newCombo) flags &= ~HitObjectFlags.LowProbability;

            // new combo should force note sheets to end
            if (newCombo) flags |= HitObjectFlags.ForceEndNotesheet;

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

            ForceStartNotesheet = 1 << 8,
            ForceEndNotesheet = 1 << 9,
            SuggestStartNotesheet = 1 << 10,
            SuggestEndNotesheet = 1 << 11,

            AllowSawbladeAddOrReplace = AllowSawbladeAdd | AllowSawbladeReplace,
        }
    }
}
