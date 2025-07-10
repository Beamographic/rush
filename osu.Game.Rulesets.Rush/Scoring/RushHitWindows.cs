// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class RushHitWindows : HitWindows
    {
        private readonly DifficultyRange greatWindowRange = new DifficultyRange(80, 50, 20);
        private readonly DifficultyRange goodWindowRange = new DifficultyRange(160, 120, 80);
        private readonly DifficultyRange missWindowRange = new DifficultyRange(200, 180, 160);

        private double great;
        private double good;
        private double miss;

        public override void SetDifficulty(double difficulty)
        {
            great = IBeatmapDifficultyInfo.DifficultyRange(difficulty, greatWindowRange);
            good = IBeatmapDifficultyInfo.DifficultyRange(difficulty, goodWindowRange);
            miss = IBeatmapDifficultyInfo.DifficultyRange(difficulty, missWindowRange);
        }

        public override double WindowFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                    return great;

                case HitResult.Good:
                    return good;

                case HitResult.Miss:
                    return miss;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        public override bool IsHitResultAllowed(HitResult result) =>
            result switch
            {
                HitResult.Miss => true,
                HitResult.Good => true,
                HitResult.Great => true,
                _ => false
            };
    }
}
