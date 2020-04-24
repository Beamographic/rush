// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Dash.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableMinion : DrawableLanedHit<Minion>
    {
        private readonly Random random = new Random();
        private readonly TextureAnimation normalAnimation;
        private readonly TextureAnimation hitAnimation;

        [Resolved]
        private DashPlayfield playfield { get; set; }

        public DrawableMinion(Minion hitObject)
            : base(hitObject)
        {
            Size = new Vector2(DashPlayfield.HIT_TARGET_SIZE);

            Content.AddRange(new Drawable[]
            {
                normalAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    FillMode = FillMode.Fit,
                    RelativeSizeAxes = Axes.Both,
                    DefaultFrameLength = 250,
                    // Size = new Vector2(DashPlayfield.HIT_TARGET_SIZE)
                },
                hitAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    FillMode = FillMode.Fit,
                    RelativeSizeAxes = Axes.Both,
                    DefaultFrameLength = 250,
                    // Size = new Vector2(DashPlayfield.HIT_TARGET_SIZE),
                    Alpha = 0f
                }
            });

            normalAnimation.IsPlaying = true;
            hitAnimation.IsPlaying = false;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            switch (HitObject.Lane)
            {
                case LanedHitLane.Air:
                    for (int i = 1; i <= 4; i++)
                        normalAnimation.AddFrame(store.Get($"flying_minion_{i}"));
                    hitAnimation.AddFrame(store.Get("flying_minion_hit"));
                    break;

                case LanedHitLane.Ground:
                    normalAnimation.AddFrame(store.Get("ground_minion"));
                    hitAnimation.AddFrame(store.Get("ground_minion_hit"));
                    break;
            }
        }

        protected override void Update()
        {
            base.Update();

            float fraction = (float)(HitObject.StartTime - Clock.CurrentTime) / 500f;
            normalAnimation.Y = (float)(Math.Sin(fraction * 2 * Math.PI) * (HitObject.Lane == LanedHitLane.Air ? 5f : 3f));
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            normalAnimation.Show();
            hitAnimation.Hide();
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Idle:
                    UnproxyContent();
                    break;

                case ArmedState.Miss:
                    this.FadeOut(300);
                    break;

                case ArmedState.Hit:
                    ProxyContent();

                    playfield.PlayerSprite.PlayAttackOnLane(HitObject.Lane);

                    normalAnimation.Hide();
                    hitAnimation.Show();

                    const float gravity_time = 300;
                    float randomness = -0.5f + (float)random.NextDouble();
                    float rotation = -180 + randomness * 50f;
                    float gravityTravelHeight = randomness * 50f + (HitObject.Lane == LanedHitLane.Air ? 500f : 400f);

                    this.RotateTo(rotation, gravity_time);
                    this.MoveToY(-gravityTravelHeight, gravity_time, Easing.Out)
                        .Then()
                        .MoveToY(gravityTravelHeight * 2, gravity_time * 2, Easing.In);

                    this.FadeOut(300);
                    break;
            }
        }
    }
}
