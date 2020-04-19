// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Dash.Scoring
{
    public class DashHitWindows : HitWindows
    {
        private static readonly DifficultyRange[] dash_ranges =
        {
            new DifficultyRange(HitResult.Great, 50, 35, 20),
            new DifficultyRange(HitResult.Good, 120, 80, 50),
            new DifficultyRange(HitResult.Miss, 135, 95, 70),
        };

        public override bool IsHitResultAllowed(HitResult result)
        {
            switch (result)
            {
                case HitResult.Great:
                case HitResult.Good:
                case HitResult.Miss:
                    return true;
            }

            return false;
        }

        protected override DifficultyRange[] GetRanges() => dash_ranges;
    }
}
