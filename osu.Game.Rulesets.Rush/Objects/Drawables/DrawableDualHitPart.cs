// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableDualHitPart : DrawableLanedHit<DualHitPart>
    {
        public override bool DisplayExplosion => true;

        public DrawableDualHitPart()
            : this(null)
        {
        }

        public DrawableDualHitPart(DualHitPart hitObject)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE);

            AddInternal(new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.DualHitPart), _ => new DualHitPartPiece()));
        }

        public new bool UpdateResult(bool userTriggered) => base.UpdateResult(userTriggered);

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            const float animation_time = 300f;

            switch (state)
            {
                case ArmedState.Miss:
                    this.FadeOut(animation_time);
                    break;

                case ArmedState.Hit:
                    float travelY = 400f * (HitObject.Lane == LanedHitLane.Air ? -1 : 1);

                    this.MoveToY(travelY, animation_time, Easing.Out);
                    this.FadeOut(animation_time);

                    break;
            }
        }
    }
}
