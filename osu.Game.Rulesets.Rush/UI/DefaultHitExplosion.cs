// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI
{
    public class DefaultHitExplosion : PoolableDrawable
    {
        private readonly Sprite colouredExplosion;
        private readonly Sprite whiteExplosion;

        public override bool RemoveWhenNotAlive => true;
        public override bool RemoveCompletedTransforms => false;

        public DefaultHitExplosion()
            : this(Color4.White)
        {
        }

        public DefaultHitExplosion(Color4 explosionColour, int sparkCount = 10, Color4? sparkColour = null)
        {
            Depth = 1f;
            Origin = Anchor.Centre;
            Size = new Vector2(200, 200);
            Scale = new Vector2(0.9f + RNG.NextSingle() * 0.2f);

            InternalChildren = new Drawable[]
            {
                colouredExplosion = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = explosionColour,
                    Scale = new Vector2(1f)
                },
                whiteExplosion = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(0.75f)
                },
                new Sparks(sparkCount)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(2f),
                    Colour = sparkColour ?? Color4.White
                }
                // TODO: stars
            };
        }

        public void Apply(DrawableHitObject HitObject)
        {
            if (HitObject is DrawableMiniBoss miniBoss)
            {
                Alpha = 0;
                Depth = 0;
                Origin = Anchor.Centre;
                Anchor = miniBoss.Anchor;
                Size = new Vector2(200, 200);
                Scale = new Vector2(0.9f + RNG.NextSingle() * 0.2f) * 1.5f;
                Rotation = RNG.NextSingle() * 360f;
                colouredExplosion.Colour = Color4.Yellow.Darken(0.5f);
            }
            else
            {
                IDrawableLanedHit laned = HitObject as IDrawableLanedHit;
                colouredExplosion.Colour = laned.LaneAccentColour;
                Anchor = laned.LaneAnchor;
                Rotation = RNG.NextSingle() * 360f;
            }
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            colouredExplosion.Texture = store.Get("Effects/explosion");
            whiteExplosion.Texture = store.Get("Effects/explosion");
        }

        protected override void PrepareForUse()
        {
            ApplyTransformsAt(double.MinValue);
            ClearTransforms();
            ApplyExplosionTransforms();
        }

        /// <summary>
        /// Applies all transforms to animate the explosion.
        /// </summary>
        protected virtual void ApplyExplosionTransforms()
        {
            this.ScaleTo(Scale * 0.5f, RushPlayfield.HIT_EXPLOSION_DURATION)
                .FadeOutFromOne(RushPlayfield.HIT_EXPLOSION_DURATION)
                .Expire(true);
        }

        protected class Sparks : CompositeDrawable
        {
            private const double average_duration = 1500f;

            private readonly Random random = new Random();
            private readonly Triangle[] triangles;

            private double randomDirection(int index, int max)
            {
                var offset = random.NextDouble() * 2f / max;
                return (double)index / max + offset;
            }

            public Sparks(int sparkCount)
            {
                Origin = Anchor.Centre;
                Anchor = Anchor.Centre;
                RelativeSizeAxes = Axes.Both;

                triangles = Enumerable.Range(0, sparkCount).Select(i => new Triangle
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Size = new Vector2(5f, 10f),
                    Rotation = (float)(randomDirection(i, sparkCount) * 360),
                }).ToArray();

                InternalChildren = triangles;
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                foreach (var triangle in triangles)
                {
                    var scale = 0.8f + random.NextDouble() * 0.2f;
                    var duration = average_duration * (0.8f + random.NextDouble() * 0.4f);
                    var radians = MathUtils.DegreesToRadians(triangle.Rotation + 90);
                    var distance = DrawWidth * (0.8f + random.NextDouble() * 0.2f);
                    var target = new Vector2(MathF.Cos(radians), MathF.Sin(radians)) * (float)distance;
                    triangle.Scale = new Vector2((float)scale);
                    triangle.MoveTo(target, duration, Easing.OutExpo);
                    triangle.FadeOutFromOne(duration, Easing.InExpo);
                }
            }
        }
    }
}
