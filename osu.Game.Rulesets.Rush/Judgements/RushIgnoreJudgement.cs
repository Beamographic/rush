// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Judgements
{
    public class RushIgnoreJudgement : RushJudgement
    {
        public override HitResult MaxResult => HitResult.IgnoreHit;

        protected override double HealthPointIncreaseFor(HitResult result, bool playerCollided) => 0.0;
    }
}
