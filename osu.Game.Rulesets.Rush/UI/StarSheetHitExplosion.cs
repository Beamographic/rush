// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class StarSheetHitExplosion : PoolableDrawable
    {
        private readonly StarSheetCapStarPiece explosionStar;
        private readonly Circle flashCircle;

        public StarSheetHitExplosion()
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.CentreLeft;

            InternalChildren = new Drawable[]
            {
                    explosionStar = new StarSheetCapStarPiece(),
                    flashCircle = new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 0.4f,
                        RelativeSizeAxes = Axes.Both,
                        Scale = new Vector2(0.5f),
                    }
            };
        }

        public void Apply(Drawable drawable)
        {
            IDrawableLanedHit laned = (IDrawableLanedHit)drawable;
            Size = drawable.Size;
            flashCircle.Colour = laned.LaneAccentColour.Lighten(0.5f);
        }

        protected override void PrepareForUse()
        {
            base.PrepareForUse();

            explosionStar.ScaleTo(1)
                         .ScaleTo(2f, RushPlayfield.HIT_EXPLOSION_DURATION)
                         .FadeOutFromOne(RushPlayfield.HIT_EXPLOSION_DURATION)
                         .Expire(true);

            flashCircle.ScaleTo(0.5f).FadeTo(0.4f)
                       .ScaleTo(4f, RushPlayfield.HIT_EXPLOSION_DURATION / 2)
                       .Then()
                       .ScaleTo(0.5f, RushPlayfield.HIT_EXPLOSION_DURATION / 2)
                       .FadeOut(RushPlayfield.HIT_EXPLOSION_DURATION / 2)
                       .Expire(true);

            this.Delay(RushPlayfield.HIT_EXPLOSION_DURATION).Expire(true);
        }
    }
}
