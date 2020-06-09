// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI.Ground
{
    public class DefaultGround : CompositeDrawable
    {
        private static readonly Color4 platform_colour = Color4.Gray.Opacity(0.2f);

        private static readonly ColourInfo slat_colour = ColourInfo.GradientVertical(Color4.Gray.Opacity(0.8f), Color4.Gray.Opacity(0.4f));
        private static readonly Vector2 slat_size = new Vector2(10f, 100f);
        private const float slat_angle = -40f;

        private const float slats_spacing = 100f;

        private static readonly Color4 ground_colour = Color4.Gray.Opacity(0.2f);

        private readonly FillFlowContainer slatsFlow;

        public DefaultGround()
        {
            AutoSizeAxes = Axes.X;
            RelativeSizeAxes = Axes.Y;

            InternalChild = new FillFlowContainer
            {
                AutoSizeAxes = Axes.X,
                RelativeSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
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
                        AutoSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = platform_colour,
                            },
                            slatsFlow = new FillFlowContainer
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

        protected override void LoadComplete()
        {
            base.LoadComplete();

            // FIXME: this is hacky but serves its purpose with no impact.
            // should add required slats on size invalidations from parent.
            const int maximum_slat_count = 125;

            for (int i = 0; i < maximum_slat_count; i++)
            {
                slatsFlow.Add(new Box
                {
                    Colour = slat_colour,
                    Size = slat_size,
                    Rotation = slat_angle,
                    Margin = new MarginPadding { Right = slats_spacing, Bottom = -10f },
                });
            }
        }
    }
}
