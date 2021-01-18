// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Judgements;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableRushJudgement : DrawableJudgement
    {
        private const float judgement_time = 250f;
        private const float judgement_movement = 300;

        public DrawableRushJudgement(JudgementResult result, DrawableRushHitObject judgedObject)
            : base(result, judgedObject)
        {
        }

        protected override void ApplyHitAnimations() =>
            this.ScaleTo(1f, judgement_time)
                .Then()
                .MoveToOffset(new Vector2(-judgement_movement, 0f), judgement_time, Easing.In)
                .Expire();

        protected override void ApplyMissAnimations() =>
            this.ScaleTo(1f, judgement_time)
                .Then()
                .MoveToOffset(new Vector2(-judgement_movement, 0f), judgement_time, Easing.In)
                .Expire();
    }
}
