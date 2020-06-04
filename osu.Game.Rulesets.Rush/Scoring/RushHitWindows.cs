// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class RushHitWindows : HitWindows
    {
        protected override DifficultyRange[] GetRanges() => new[]
        {
            new DifficultyRange(HitResult.Perfect, 22.4D, 19.4D, 13.9D),
            new DifficultyRange(HitResult.Great, 127, 112, 97),
            new DifficultyRange(HitResult.Miss, 188, 173, 158),
        };

        public override bool IsHitResultAllowed(HitResult result) =>
            result switch
            {
                HitResult.Miss => true,
                HitResult.Great => true,
                HitResult.Perfect => true,
                _ => false
            };
    }
}
