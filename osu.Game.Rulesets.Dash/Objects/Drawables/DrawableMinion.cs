// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableMinion : DrawableLanedHit<Minion>
    {
        private readonly Random random = new Random();
        private readonly Sprite sprite;

        public DrawableMinion(Minion hitObject)
            : base(hitObject)
        {
            AddInternal(sprite = new Sprite
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                Scale = new Vector2(3f),
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            sprite.Texture = store.Get("cirno");
        }

        protected override void Update()
        {
            base.Update();

            float fraction = (float)(HitObject.StartTime - Clock.CurrentTime) / 500f;
            sprite.Y = (float)(Math.Sin(fraction * 2 * Math.PI) * 5f);
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
