// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    [Cached]
    public class RushPlayfield : ScrollingPlayfield
    {
        public const float DEFAULT_HEIGHT = 178;
        public const float HIT_TARGET_OFFSET = 120;
        public const float HIT_TARGET_SIZE = 100;
        public const float PLAYER_OFFSET = 100;

        public RushPlayerSprite PlayerSprite { get; }

        internal readonly Container EffectContainer;

        public RushPlayfield()
        {
            InternalChildren = new Drawable[]
            {
                new Container
                {
                    Name = "Right Area",
                    RelativeSizeAxes = Axes.X,
                    Size = new Vector2(1, DEFAULT_HEIGHT),
                    Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
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
                                new HitTarget
                                {
                                    Anchor = Anchor.TopLeft,
                                    Origin = Anchor.Centre,
                                    Size = new Vector2(HIT_TARGET_SIZE),
                                    FillMode = FillMode.Fit
                                },
                                new HitTarget
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
                        EffectContainer = new Container
                        {
                            Name = "Effects",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET }
                        }
                    }
                },
                new Container
                {
                    Name = "Left Area",
                    RelativeSizeAxes = Axes.Y,
                    Size = new Vector2(HIT_TARGET_OFFSET, 1),
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
                            PlayerSprite = new RushPlayerSprite(DEFAULT_HEIGHT, 0)
                            {
                                Origin = Anchor.Centre,
                                Position = new Vector2(PLAYER_OFFSET, DEFAULT_HEIGHT),
                                Scale = new Vector2(0.75f),
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
