// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Judgements
{
    public class RushFeverJudgement : RushJudgement
    {
        public override HitResult MaxResult => HitResult.LargeBonus;

        protected override double HealthPointIncreaseFor(HitResult result, bool collided) => 0;
    }
}
