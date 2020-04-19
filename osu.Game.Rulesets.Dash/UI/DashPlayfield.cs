// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;

namespace osu.Game.Rulesets.Dash.UI
{
    [Cached]
    public class DashPlayfield : ScrollingPlayfield
    {
        public const float DEFAULT_HEIGHT = 178;
        public const float HIT_TARGET_OFFSET = 100;
        public const float HIT_TARGET_SIZE = 100;
        private const float left_area_size = 100;

        private readonly Sprite playerSprite;
        private readonly HitTarget hitTargetAir;
        private readonly HitTarget hitTargetGround;

        public DashPlayfield()
        {
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    Name = "Right Area",
                    RelativeSizeAxes = Axes.X,
                    Size = new Vector2(1, DEFAULT_HEIGHT),
                    Padding = new MarginPadding { Left = left_area_size },
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Children = new Drawable[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                            Children = new Drawable[]
                            {
                                hitTargetAir = new HitTarget
                                {
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.Centre,
                                    Size = new Vector2(HIT_TARGET_SIZE),
                                    FillMode = FillMode.Fit
                                },
                                hitTargetGround = new HitTarget
                                {
                                    Anchor = Anchor.BottomLeft,
                                    Origin = Anchor.Centre,
                                    Size = new Vector2(HIT_TARGET_SIZE),
                                    FillMode = FillMode.Fit
                                },
                            }
                        },
                        new Container
                        {
                            Name = "Hit Objects",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                            Child = HitObjectContainer
                        },
                    }
                },
                new Container
                {
                    Name = "Left Area",
                    RelativeSizeAxes = Axes.Y,
                    Size = new Vector2(left_area_size, 1),
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Child = new Container
                    {
                        Name = "Left Play Zone",
                        RelativeSizeAxes = Axes.X,
                        Size = new Vector2(1, DEFAULT_HEIGHT),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        Children = new Drawable[]
                        {
                            playerSprite = new DashPlayerSprite(DEFAULT_HEIGHT, 0)
                            {
                                Origin = Anchor.Centre,
                                Position = new Vector2(100, DEFAULT_HEIGHT),
                                Scale = new Vector2(2f),
                            },
                        }
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
        }
    }
}
