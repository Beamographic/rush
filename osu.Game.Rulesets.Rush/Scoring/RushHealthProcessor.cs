// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class RushHealthProcessor : HealthProcessor
    {
        public double PlayerHealthPercentage { get; }

        private double healthForPoints(double points) => points / PlayerHealthPercentage;

        [Resolved]
        private DrawableRushRuleset drawableRushRuleset { get; set; }

        public RushHealthProcessor(double playerHealthPercentage = 100f)
        {
            PlayerHealthPercentage = playerHealthPercentage;
        }

        protected override double GetHealthIncreaseFor(JudgementResult result)
        {
            var healthAmount = result.Judgement is RushJudgement rushJudgement ? healthForPoints(rushJudgement.HealthPoints) : 0;

            return result.HitObject switch
            {
                // requires hit
                Heart _ when result.IsHit => healthAmount,
                // requires not hit
                Sawblade _ when !result.IsHit => healthAmount,
                MiniBoss _ when !result.IsHit => healthAmount,
                // requires collision
                Minion _ when !result.IsHit && collidesWith(result.HitObject) => healthAmount,
                Orb _ when !result.IsHit && collidesWith(result.HitObject) => healthAmount,
                // default
                _ => 0
            };
        }

        private bool collidesWith(HitObject hitObject) => drawableRushRuleset.Playfield.PlayerSprite.CollidesWith(hitObject);
    }
}
