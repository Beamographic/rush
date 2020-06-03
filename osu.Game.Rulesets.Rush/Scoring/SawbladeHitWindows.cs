// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class SawbladeHitWindows : RushHitWindows
    {
        protected override DifficultyRange[] GetRanges() => new[]
        {
            new DifficultyRange(HitResult.Perfect, 20, 20, 20),
            new DifficultyRange(HitResult.Miss, 400, 400, 400)
        };

        public override bool IsHitResultAllowed(HitResult result) => result == HitResult.Perfect || result == HitResult.Miss;
    }
}
