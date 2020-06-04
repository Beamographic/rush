// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class RushHealthProcessor : HealthProcessor
    {
        private const float sawblade_points = -20f;
        private const float minion_points = -10f;
        private const float orb_points = -10f;
        private const float miniboss_points = -10f;
        private const float heart_points = 50f;

        public double PlayerHealthPercentage { get; }

        private double healthForPoints(double points) => points / PlayerHealthPercentage;

        [Resolved]
        private DrawableRushRuleset drawableRushRuleset { get; set; }

        public RushHealthProcessor(double playerHealthPercentage = 100f)
        {
            PlayerHealthPercentage = playerHealthPercentage;
        }

        protected override double GetHealthIncreaseFor(JudgementResult result) =>
            result.HitObject switch
            {
                Heart _ when result.IsHit => healthForPoints(heart_points),
                Sawblade _ when !result.IsHit => healthForPoints(sawblade_points),
                Minion _ when !result.IsHit && collidesWith(result.HitObject) => healthForPoints(minion_points),
                Orb _ when !result.IsHit && collidesWith(result.HitObject) => healthForPoints(orb_points),
                MiniBoss _ when !result.IsHit => healthForPoints(miniboss_points),
                _ => 0
            };

        private bool collidesWith(HitObject hitObject) => drawableRushRuleset.Playfield.PlayerSprite.CollidesWith(hitObject);
    }
}
