// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace osu.Game.Rulesets.Rush.UI.Ground
{
    /// <summary>
    /// Represents a component displaying the ground the player will be standing on.
    /// </summary>
    public class GroundDisplay : CompositeDrawable
    {
        private const double scroll_speed = 1f;

        private readonly FillFlowContainer groundFlow;

        public GroundDisplay()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChildren = new[]
            {
                groundFlow = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new DefaultGround { Name = "Leaving-out piece" },
                        new DefaultGround { Name = "Coming-in piece" },
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            ScheduleAfterChildren(() =>
            {
                var pieceWidth = groundFlow.Width / 2;

                groundFlow.MoveToX(-pieceWidth, pieceWidth / scroll_speed)
                          .Then()
                          .MoveToX(0f)
                          .Loop(1f);
            });
        }
    }
}
