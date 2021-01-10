// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Judgements
{
    public class RushJudgement : Judgement
    {
        /// <summary>
        /// Retrieves the numeric health point increase of a <see cref="RushJudgementResult"/>.
        /// </summary>
        /// <param name="result">The <see cref="RushJudgementResult"/> to find the numeric health points increase for.</param>
        public double HealthPointIncreaseFor(RushJudgementResult result) => HealthPointIncreaseFor(result.Type, result.PlayerCollided);

        /// <summary>
        /// Retrieves the numeric health point increase of a <see cref="HitResult"/> and whether the player collided.
        /// </summary>
        /// <param name="result">The <see cref="HitResult"/> to find the numeric health points increase for.</param>
        /// <param name="playerCollided">Whether the player collided with the corresponding hit object at the point of judgement.</param>
        /// <returns>The numeric health points increase of <paramref name="result"/> and <paramref name="playerCollided"/>.</returns>
        protected virtual double HealthPointIncreaseFor(HitResult result, bool playerCollided) => 0.0;

        protected sealed override double HealthIncreaseFor(HitResult result) => throw new NotSupportedException($"Use the Rush!-specific version: {nameof(HealthPointIncreaseFor)}");
    }
}
