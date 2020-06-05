// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableMinion : DrawableLanedHit<Minion>
    {
        private readonly Random random = new Random();
        private readonly TextureAnimation normalAnimation;
        private readonly TextureAnimation hitAnimation;

        [Resolved]
        private RushPlayfield playfield { get; set; }

        public DrawableMinion(Minion hitObject)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE);

            Content.AddRange(new Drawable[]
            {
                normalAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    FillMode = FillMode.Fit,
                    RelativeSizeAxes = Axes.Both,
                    DefaultFrameLength = 250,
                },
                hitAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    FillMode = FillMode.Fit,
                    RelativeSizeAxes = Axes.Both,
                    DefaultFrameLength = 250,
                    Alpha = 0f
                }
            });

            normalAnimation.IsPlaying = true;
            hitAnimation.IsPlaying = false;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            string lane = HitObject.Lane == LanedHitLane.Air ? "air" : "ground";
            normalAnimation.AddFrames(new[] { store.Get($"Minion/pippidon_{lane}_0"), store.Get($"Minion/pippidon_{lane}_0") });
            hitAnimation.AddFrame(store.Get($"Minion/pippidon_{lane}_hit"));
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
            base.UpdateStateTransforms(state);

            switch (state)
            {
                case ArmedState.Hit:
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
