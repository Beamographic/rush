// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class SawbladeHitWindows : RushHitWindows
    {
        private readonly DifficultyRange greatWindowRange = new DifficultyRange(20, 20, 20);
        private readonly DifficultyRange missWindowRange = new DifficultyRange(50, 50, 50);

        private double great;
        private double miss;

        public override void SetDifficulty(double difficulty)
        {
            great = IBeatmapDifficultyInfo.DifficultyRange(difficulty, greatWindowRange);
            miss = IBeatmapDifficultyInfo.DifficultyRange(difficulty, missWindowRange);
        }

        public override double WindowFor(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                    return great;

                case HitResult.Miss:
                    return miss;

                default:
                    throw new ArgumentOutOfRangeException(nameof(result), result, null);
            }
        }

        public override bool IsHitResultAllowed(HitResult result) => result == HitResult.Great || result == HitResult.Miss;
    }
}
