// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Judgements
{
    public class RushJudgement : Judgement
    {
        protected override int NumericResultFor(HitResult result) =>
            result switch
            {
                HitResult.Great => 200,
                HitResult.Perfect => 300,
                _ => 0
            };

        public virtual double HealthPoints => -10;
    }
}
