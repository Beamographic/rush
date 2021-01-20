// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Judgements
{
    /// <summary>
    /// A judgement that decreases the player health points when
    /// the player collides with the hit object without hitting it.
    /// </summary>
    public class CollisionDamagingJudgement : RushJudgement
    {
        protected override double HealthPointIncreaseFor(HitResult result, bool playerCollided) =>
            result == MinResult && playerCollided ? -10.0 : 0.0;
    }
}
