// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class DrawableRushJudgement : DrawableJudgement
    {
        [Resolved]
        private RushPlayfield playfield { get; set; }

        public DrawableRushJudgement(JudgementResult result, DrawableHitObject judgedObject)
            : base(result, judgedObject)
        {
        }

        public DrawableRushJudgement()
        {
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            Origin = Anchor.Centre;
            Alpha = 1f;

            if (JudgedObject is DrawableSawblade drawableSawblade)
            {
                Position = new Vector2(0f, playfield.JudgementPositionForLane(drawableSawblade.Lane.Opposite()));
                Scale = new Vector2(1.2f);
            }
            else if (JudgedObject is IDrawableLanedHit drawableLanedHit)
            {
                Position = new Vector2(0f, playfield.JudgementPositionForLane(drawableLanedHit.Lane));
                Scale = new Vector2(1.5f);
            }
            else
            {
                Position = Vector2.Zero;
                Scale = Vector2.One;
            }
        }

        protected override void ApplyHitAnimations()
        {
            const float judgement_time = 250f;
            const float judgement_movement = 300f;

            this.ScaleTo(1f, judgement_time)
                .Then()
                .MoveToOffset(new Vector2(-judgement_movement, 0f), judgement_time, Easing.In)
                .FadeOut(200f);

            LifetimeEnd = LatestTransformEndTime;

            base.ApplyHitAnimations();
        }
    }
}
