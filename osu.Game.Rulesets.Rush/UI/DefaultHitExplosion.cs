// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI
{
    public class DefaultHitExplosion : CompositeDrawable
    {
        private readonly Sprite colouredExplosion;
        private readonly Sprite whiteExplosion;

        public override bool RemoveWhenNotAlive => true;

        public DefaultHitExplosion(Color4 explosionColour, int sparkCount = 10, Color4? sparkColour = null)
        {
            Origin = Anchor.Centre;

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

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            colouredExplosion.Texture = store.Get("exp");
            whiteExplosion.Texture = store.Get("exp");
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
