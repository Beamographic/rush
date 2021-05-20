// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.UI.Scrolling;

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
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.TopCentre;
            RelativeSizeAxes = Axes.Both;
            Padding = new MarginPadding { Top = 50f, Left = RushPlayfield.HIT_TARGET_OFFSET };

            InternalChildren = new[]
            {
                groundFlow = new FillFlowContainer
                {
                    AutoSizeAxes = Axes.X,
                    RelativeSizeAxes = Axes.Y,
                   // RelativePositionAxes = Axes.X,
                    Direction = FillDirection.Horizontal,
                    Children = new Drawable[]
                    {
                        new DefaultGround { Name = "Leaving-out piece" },
                        new DefaultGround { Name = "Coming-in piece" },
                    }
                }
            };
        }

        [Resolved(canBeNull: true)]
        private IScrollingInfo scrollingInfo { get; set; }

        private double scrollingRange => scrollingInfo.TimeRange.Value;

        private double? startTime;

        protected override void UpdateAfterChildren()
        {
            base.UpdateAfterChildren();

            if (!startTime.HasValue)
                startTime = Time.Current;

            if (scrollingInfo is null) return;

            groundFlow.X = scrollingInfo.Algorithm.PositionAt(0f, Time.Current, scrollingInfo.TimeRange.Value, DrawWidth) % (groundFlow.Width / 2f);
        }
    }
}
