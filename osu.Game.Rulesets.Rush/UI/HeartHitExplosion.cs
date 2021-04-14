// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class HeartHitExplosion : HeartPiece
    {
        public override bool RemoveCompletedTransforms => false;

        public HeartHitExplosion()
        {
            Origin = Anchor.Centre;
            Scale = new Vector2(0.5f);
        }

        protected override void PrepareForUse()
        {
            ApplyTransformsAt(double.MinValue);
            ClearTransforms();

            this.ScaleTo(1.25f, RushPlayfield.HIT_EXPLOSION_DURATION)
                .FadeOutFromOne(RushPlayfield.HIT_EXPLOSION_DURATION)
                .Expire(true);
        }

        public void Apply(DrawableHeart drawableHeart)
        {
            Anchor = drawableHeart.LaneAnchor;
            Size = drawableHeart.Size;
        }
    }
}
