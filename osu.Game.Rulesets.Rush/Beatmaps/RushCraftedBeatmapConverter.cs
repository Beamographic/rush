// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Audio;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    /// <summary>
    /// Converts a specially crafted osu! beatmap to Rush! format.
    /// This is a temporary measure until the lazer editor supports selecting rulesets.
    /// </summary>
    ///
    /// <remarks>
    ///
    /// The playfield is split into the following sections:
    ///
    /// <code>
    /// 0  170 340  512
    /// +---+---+---+ 0
    /// | A | D | F |
    /// +---+   |   | 160
    /// | B +---+---+ 192
    /// +---+   |   | 224
    /// | C | E | G |
    /// +---+---+---+ 384
    ///</code>
    ///
    /// Hitobjects may appear anywhere in these sections, but the following positions are suggested
    /// to make a .osu file more human-readable:
    /// A: 100,100
    /// B: 100,200
    /// C: 100,300
    /// D: 250,100
    /// E: 250,300
    /// F: 400,100
    /// G: 400,300
    ///
    /// Hitcircles:
    /// 1) Hitcircles in section A or C are by default "air" or "ground" minions respectively
    /// 2) Hitcircles in section B are "dual hits"
    /// 3) Hitcircles in section D or E are by default "air" or "ground" sawblades respectively
    /// 4) Hitcircles in section F or G are by default "air" or "ground" hammers respectively (TODO)
    ///
    /// Hitsounds for hitcircles in A and C:
    /// 1) The "whistle" hitsound will be converted to medium minions (TODO)
    /// 2) The "clap" hitsound will be converted to large minions (TODO)
    ///
    /// Hitsounds for hitcircles in D and E:
    /// 1) The "finish" hitsound will be converted to hearts rather than sawblades.
    ///
    /// Sliders:
    /// 1) Sliders that start in section A or C are by default "air" or "ground" notesheets respectively
    /// 2) Sliders that start in section B will add equal length notesheets to both air and ground
    /// 3) Only the slider start is checked when selecting a lane
    /// 4) Slider distance and repeats are ignored, only the final end time is used
    ///
    /// Spinners:
    /// 1) Spinners are always "minibosses"
    ///
    /// Misc:
    /// 1) Slider velocity and BPM are currently ignored, but may be used in the future
    /// 2) Kiai will be used in the future to indicate boss sections
    /// 3) New combo is ignored
    /// 4) Hitsounds are only transferred for minions
    /// </remarks>
    public class RushCraftedBeatmapConverter : BeatmapConverter<RushHitObject>
    {
        public RushCraftedBeatmapConverter(IBeatmap beatmap, Ruleset ruleset)
            : base(beatmap, ruleset)
        {
        }

        public override bool CanConvert() => true;

        protected override IEnumerable<RushHitObject> ConvertHitObject(HitObject original, IBeatmap beatmap)
        {
            if (!typeForObject(original, out var hitObjectType, out var lane, out var minionSize))
                yield break;

            switch (hitObjectType)
            {
                case HitObjectType.Hammer: // TODO: hammers
                case HitObjectType.Minion:
                    // TODO: use minion size
                    yield return new Minion
                    {
                        StartTime = original.StartTime,
                        Samples = original.Samples,
                        Lane = lane
                    };

                    break;

                case HitObjectType.DualNoteSheet:
                case HitObjectType.NoteSheet:
                    if (hitObjectType == HitObjectType.DualNoteSheet || lane == LanedHitLane.Air)
                    {
                        yield return new NoteSheet
                        {
                            StartTime = original.StartTime,
                            EndTime = original.GetEndTime(),
                            Lane = LanedHitLane.Air
                        };
                    }

                    if (hitObjectType == HitObjectType.DualNoteSheet || lane == LanedHitLane.Ground)
                    {
                        yield return new NoteSheet
                        {
                            StartTime = original.StartTime,
                            EndTime = original.GetEndTime(),
                            Lane = LanedHitLane.Ground
                        };
                    }

                    break;

                case HitObjectType.DualHit:
                    yield return new DualHit
                    {
                        StartTime = original.StartTime
                    };

                    break;

                case HitObjectType.MiniBoss:
                    yield return new MiniBoss
                    {
                        StartTime = original.StartTime,
                        EndTime = original.GetEndTime()
                    };

                    break;

                case HitObjectType.Sawblade:
                    yield return new Sawblade
                    {
                        StartTime = original.StartTime,
                        Lane = lane
                    };

                    break;

                case HitObjectType.Heart:
                    yield return new Heart
                    {
                        StartTime = original.StartTime,
                        Lane = lane
                    };

                    break;
            }
        }

        private bool typeForObject(HitObject hitObject, out HitObjectType hitObjectType, out LanedHitLane lane, out MinionSize minionSize)
        {
            const float vertical_left = 170f;
            const float vertical_right = 340f;
            const float horizontal_top = 160f;
            const float horizontal_middle = 192f;
            const float horizontal_bottom = 224f;

            bool hasClap() => hitObject.Samples.Any(s => s.Name == HitSampleInfo.HIT_CLAP);
            bool hasFinish() => hitObject.Samples.Any(s => s.Name == HitSampleInfo.HIT_FINISH);
            bool hasWhistle() => hitObject.Samples.Any(s => s.Name == HitSampleInfo.HIT_WHISTLE);

            hitObjectType = HitObjectType.Minion;
            lane = LanedHitLane.Air;
            minionSize = MinionSize.Small;

            // this should never happen, honestly
            if (!(hitObject is IHasPosition position))
                return false;

            if (hitObject is IHasDuration && !(hitObject is IHasDistance))
            {
                hitObjectType = HitObjectType.MiniBoss;
                return true;
            }

            if (position.X < vertical_left)
            {
                if (position.Y >= horizontal_top && position.Y < horizontal_bottom)
                {
                    hitObjectType = hitObject is IHasDuration ? HitObjectType.DualNoteSheet : HitObjectType.DualHit;
                    return true;
                }

                lane = position.Y < horizontal_top ? LanedHitLane.Air : LanedHitLane.Ground;

                if (hitObject is IHasDuration)
                    hitObjectType = HitObjectType.NoteSheet;
                else
                {
                    hitObjectType = HitObjectType.Minion;

                    if (hasWhistle())
                        minionSize = MinionSize.Medium;
                    else if (hasClap())
                        minionSize = MinionSize.Large;
                }

                return true;
            }

            lane = position.Y < horizontal_middle ? LanedHitLane.Air : LanedHitLane.Ground;

            if (position.X >= vertical_right)
            {
                hitObjectType = HitObjectType.Hammer;
                return true;
            }

            hitObjectType = hasFinish() ? HitObjectType.Heart : HitObjectType.Sawblade;

            return true;
        }

        private enum HitObjectType
        {
            Minion,
            NoteSheet,
            DualNoteSheet, // helper to save on double sliders
            DualHit,
            MiniBoss,
            Sawblade,
            Hammer,
            Heart
        }

        private enum MinionSize
        {
            Small,
            Medium,
            Large
        }
    }
}
