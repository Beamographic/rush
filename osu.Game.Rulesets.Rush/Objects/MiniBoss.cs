// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Threading;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class MiniBoss : RushHitObject, IHasDuration
    {
        public static readonly int DEFAULT_REQUIRED_HITS_PER_SECOND = 3;

        public double EndTime
        {
            get => StartTime + Duration;
            set => Duration = value - StartTime;
        }

        public double Duration { get; set; }

        public int RequiredHitsPerSecond => DEFAULT_REQUIRED_HITS_PER_SECOND;

        public int RequiredHits => (int)(Math.Ceiling(Duration / 500f) * RequiredHitsPerSecond * 0.5f);

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            for (int i = 0; i < RequiredHits; i++)
                AddNested(new MiniBossTick());
        }

        public override Judgement CreateJudgement() => new RushMiniBossJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
