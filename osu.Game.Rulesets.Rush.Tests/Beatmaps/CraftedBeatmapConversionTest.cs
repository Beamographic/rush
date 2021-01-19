// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using osu.Framework.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Tests.Beatmaps;

namespace osu.Game.Rulesets.Rush.Tests.Beatmaps
{
    [TestFixture]
    public class CraftedBeatmapConversionTest : BeatmapConversionTest<ConvertValue>
    {
        protected override string ResourceAssembly => typeof(RushRuleset).Namespace;

        // TODO: add tests for this
        //public void Test(string name) => base.Test(name);

        protected override IEnumerable<ConvertValue> CreateConvertValue(HitObject hitObject)
        {
            yield return new ConvertValue
            {
                StartTime = hitObject.StartTime,
                EndTime = hitObject.GetEndTime(),
                Lane = (hitObject as LanedHit)?.Lane,
                IsMinion = hitObject is Minion,
                IsStarSheet = hitObject is StarSheet,
                IsMiniBoss = hitObject is MiniBoss,
                IsSawblade = hitObject is Sawblade,
                HasHeart = hitObject is Heart /*|| (hitObject is LanedHit)?.HasHeart*/,
                IsHammer = false, // TODO
                IsNote = false, // TODO
            };
        }

        protected override Ruleset CreateRuleset() => new RushRuleset();
    }

    public struct ConvertValue : IEquatable<ConvertValue>
    {
        /// <summary>
        /// A sane value to account for osu!stable using ints everywhere.
        /// </summary>
        private const float conversion_lenience = 2;

        public double StartTime;
        public double EndTime;
        public LanedHitLane? Lane;
        public bool IsMinion;
        public bool IsStarSheet;
        public bool IsMiniBoss;
        public bool IsSawblade;
        public bool IsHammer;
        public bool HasHeart;
        public bool IsNote;

        public bool Equals(ConvertValue other)
            => Precision.AlmostEquals(StartTime, other.StartTime, conversion_lenience)
               && Precision.AlmostEquals(EndTime, other.EndTime, conversion_lenience)
               && Lane == other.Lane
               && IsMinion == other.IsMinion
               && IsStarSheet == other.IsStarSheet
               && IsSawblade == other.IsSawblade
               && IsHammer == other.IsHammer
               && HasHeart == other.HasHeart
               && IsNote == other.IsNote
               && IsMiniBoss == other.IsMiniBoss;
    }
}
