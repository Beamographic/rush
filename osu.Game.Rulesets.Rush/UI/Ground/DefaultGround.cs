// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Layout;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI.Ground
{
    public partial class DefaultGround : CompositeDrawable
    {
        private static readonly Color4 platform_colour = Color4.Gray.Opacity(0.2f);

        private static readonly ColourInfo slat_colour = ColourInfo.GradientVertical(Color4.Gray.Opacity(0.8f), Color4.Gray.Opacity(0.4f));
        private static readonly Vector2 slat_size = new Vector2(10f, 100f);

        private const float slat_angle = -40f;

        private const float slats_spacing = 100f;

        private static readonly Color4 ground_colour = Color4.Gray.Opacity(0.2f);

        private readonly Container slats;

        private readonly DrawablePool<GroundLine> linePool;

        public DefaultGround()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    linePool = new DrawablePool<GroundLine>(10),
                    new Box
                    {
                        Name = "Top line",
                        Colour = platform_colour.Opacity(1f).Lighten(1f),
                        RelativeSizeAxes = Axes.X,
                        Height = 3f,
                        Depth = -1,
                    },
                    new Container
                    {
                        Name = "Platform",
                        Masking = true,
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = platform_colour,
                            },
                            slats = new Container
                            {
                                AutoSizeAxes = Axes.Both,
                            },
                        }
                    },
                    new Box
                    {
                        Name = "Bottom line",
                        Colour = platform_colour.Opacity(1f).Lighten(2f),
                        RelativeSizeAxes = Axes.X,
                        Height = 3f,
                    },
                    new Box
                    {
                        Name = "Ground",
                        Colour = ground_colour,
                        RelativeSizeAxes = Axes.Both,
                    }
                }
            };
        }

        [Resolved(canBeNull: true)]
        private IScrollingInfo scrollingInfo { get; set; }

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            // Tests don't have scrolling info yet
            if (scrollingInfo is null) return;

            var groundX = scrollingInfo.Algorithm.Value.PositionAt(0f, Time.Current, scrollingInfo.TimeRange.Value, DrawWidth - RushPlayfield.HIT_TARGET_OFFSET) % slats_spacing;

            // This is to ensure that the ground is still visible before the start of the track
            if (groundX > 0)
                groundX = -slats_spacing + groundX;

            slats.X = groundX;
        }

        protected override bool OnInvalidate(Invalidation invalidation, InvalidationSource source)
        {
            if (invalidation.HasFlag(Invalidation.DrawSize))
            {
                slats.Clear(false);

                for (float i = 0; i < DrawWidth + slats_spacing; i += slats_spacing)
                    slats.Add(linePool.Get(l => l.X = i));
            }

            return base.OnInvalidate(invalidation, source);
        }

        private partial class GroundLine : PoolableDrawable
        {
            public override bool RemoveWhenNotAlive => false;

            public GroundLine()
            {
                Colour = slat_colour;
                Anchor = Anchor.TopLeft;
                Origin = Anchor.TopLeft;
                Size = slat_size;
                Rotation = slat_angle;
                Margin = new MarginPadding { Bottom = -10f };
                InternalChild = new Box
                {
                    RelativeSizeAxes = Axes.Both
                };
            }
        }
    }
}
