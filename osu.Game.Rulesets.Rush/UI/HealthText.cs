// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI
{
    public class HealthText : PoolableDrawable
    {
        private readonly SpriteText text;

        public override bool RemoveCompletedTransforms => false;

        public HealthText()
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.CentreLeft;

            InternalChildren = new Drawable[]
            {
                text = new SpriteText
                {
                    Font = FontUsage.Default.With(size: 40),
                    Scale = new Vector2(1.2f),
                }
            };
        }

        public void Apply(double pointDifference)
        {
            text.Colour = pointDifference > 0 ? Color4.Green : Color4.Red;
            text.Text = $"{pointDifference:+0;-0}";
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            const float judgement_time = 250f;

            ApplyTransformsAt(double.MinValue);
            ClearTransforms();

            this.ScaleTo(1f, judgement_time)
                .Then()
                .FadeOutFromOne(judgement_time)
                .MoveToOffset(new Vector2(0f, -20f), judgement_time)
                .Expire(true);
        }
    }
}
