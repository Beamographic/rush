// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Judgements
{
    public class RushJudgementResult : JudgementResult
    {
        /// <summary>
        /// The <see cref="RushHitObject"/> that was judged.
        /// </summary>
        public new RushHitObject HitObject => (RushHitObject)base.HitObject;

        /// <summary>
        /// The judgement which this <see cref="RushJudgementResult"/> applies for.
        /// </summary>
        public new RushJudgement Judgement => (RushJudgement)base.Judgement;

        /// <summary>
        /// The gathered amount of fever prior to this <see cref="RushJudgementResult"/> occurring.
        /// </summary>
        public float FeverProgressAtJudgement { get; set; }

        /// <summary>
        /// Whether the player has collided with the corresponding hit object at the point of judgement.
        /// </summary>
        public bool PlayerCollided;

        public RushJudgementResult(RushHitObject hitObject, RushJudgement judgement)
            : base(hitObject, judgement)
        {
        }

        public override string ToString() => $"{Type} (HP:{Judgement.HealthPointIncreaseFor(this)} {Judgement})";
    }
}
