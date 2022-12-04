// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public partial class RushHealthProcessor : HealthProcessor
    {
        public double PlayerHealthPercentage { get; }

        public RushHealthProcessor(double playerHealthPercentage = 100f)
        {
            PlayerHealthPercentage = playerHealthPercentage;
        }

        protected virtual double GetHealthPointIncreaseFor(RushJudgementResult result) => result.Judgement.HealthPointIncreaseFor(result);

        protected sealed override double GetHealthIncreaseFor(JudgementResult result)
        {
            var pointIncrease = GetHealthPointIncreaseFor((RushJudgementResult)result);
            return pointIncrease / PlayerHealthPercentage;
        }

        protected override JudgementResult CreateResult(HitObject hitObject, Judgement judgement) => new RushJudgementResult((RushHitObject)hitObject, (RushJudgement)judgement);
    }
}
