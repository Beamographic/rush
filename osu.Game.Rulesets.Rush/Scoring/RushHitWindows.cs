// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class RushHitWindows : HitWindows
    {
        protected override DifficultyRange[] GetRanges() => new[]
        {
            new DifficultyRange(HitResult.Great, 80, 50, 20),
            new DifficultyRange(HitResult.Good, 160, 120, 80),
            new DifficultyRange(HitResult.Miss, 200, 180, 160),
        };

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
