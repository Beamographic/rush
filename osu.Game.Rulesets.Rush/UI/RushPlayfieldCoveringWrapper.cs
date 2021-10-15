// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI
{
    /// <summary>
    /// A <see cref="Container"/> that has its contents partially hidden by an adjustable "cover". This is intended to be used in a playfield.
    /// </summary>
    public class PlayfieldCoveringWrapper : CompositeDrawable
    {
        /// <summary>
        /// The complete cover, including gradient and fill.
        /// </summary>
        private readonly Drawable cover;

        /// <summary>
        /// The gradient portion of the cover.
        /// </summary>
        private readonly Box gradient;

        /// <summary>
        /// The fully-opaque portion of the cover.
        /// </summary>
        private readonly Box filled;

        public PlayfieldCoveringWrapper(Drawable content)
        {
            InternalChild = new BufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Height = 2,
                Children = new[]
                {
                    new Container{
                        RelativeSizeAxes = Axes.Both,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Height = 0.5f,
                        Child = content,
                    },
                    cover = new Container
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Blending = new BlendingParameters
                        {
                            // Don't change the destination colour.
                            RGBEquation = BlendingEquation.Add,
                            Source = BlendingType.Zero,
                            Destination = BlendingType.One,
                            // Subtract the cover's alpha from the destination (points with alpha 1 should make the destination completely transparent).
                            AlphaEquation = BlendingEquation.Add,
                            SourceAlpha = BlendingType.Zero,
                            DestinationAlpha = BlendingType.OneMinusSrcAlpha
                        },
                        Children = new Drawable[]
                        {
                            gradient = new Box
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                RelativeSizeAxes = Axes.Both,
                                RelativePositionAxes = Axes.Both,
                                Width = 0.25f,
                                Colour = ColourInfo.GradientHorizontal(
                                    Color4.White.Opacity(1f),
                                    Color4.White.Opacity(0f)
                                )
                            },
                            filled = new Box
                            {
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                RelativeSizeAxes = Axes.Both,
                                Width = 0
                            }
                        }
                    }
                }
            };
        }


        /// <summary>
        /// The relative area that should be completely covered. This does not include the fade.
        /// </summary>
        public float Coverage
        {
            set
            {
                filled.Width = value;
                gradient.X = value;
            }
        }

        /// <summary>
        /// The direction in which the cover expands.
        /// </summary>
        public CoverExpandDirection Direction
        {
            set => cover.Scale = value == CoverExpandDirection.AlongScroll ? new Vector2(-1, 1) : Vector2.One;
        }
    }

    public enum CoverExpandDirection
    {
        /// <summary>
        /// The cover expands along the scrolling direction.
        /// </summary>
        AlongScroll,

        /// <summary>
        /// The cover expands against the scrolling direction.
        /// </summary>
        AgainstScroll
    }
}
