// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableMinion : DrawableLanedHit<Minion>
    {
        protected virtual RushSkinComponents Component => RushSkinComponents.Minion;

        private readonly Drawable minionPiece;

        private readonly Random random = new Random();

        [Resolved]
        private RushPlayfield playfield { get; set; }

        public override bool DisplayExplosion => true;

        public DrawableMinion()
            : this(null)
        {
        }

        public DrawableMinion(Minion hitObject = null)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE);

            AddInternal(minionPiece = new SkinnableDrawable(new RushSkinComponent(Component), _ => new MinionPiece())
            {
                RelativeSizeAxes = Axes.Both
            });
        }

        protected override void Update()
        {
            base.Update();

            float fraction = (float)(HitObject.StartTime - Clock.CurrentTime) / 500f;
            minionPiece.Y = (float)(Math.Sin(fraction * 2 * Math.PI) * (HitObject.Lane == LanedHitLane.Air ? 5f : 3f));
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            switch (state)
            {
                case ArmedState.Hit:
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
