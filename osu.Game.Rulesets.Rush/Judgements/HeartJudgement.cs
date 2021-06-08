// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Judgements
{
    public class HeartJudgement : RushJudgement
    {
        public override HitResult MaxResult => HitResult.SmallBonus;

        protected override double HealthPointIncreaseFor(HitResult result, bool collided) =>
            result == MinResult && !collided ? 0.0 : 50.0;
    }
}
