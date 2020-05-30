// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
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

        public override void Add(DrawableHitObject hitObject)
        {
            hitObject.OnNewResult += onNewResult;

            base.Add(hitObject);
        }

        public override bool Remove(DrawableHitObject hitObject)
        {
            if (!base.Remove(hitObject))
                return false;

            hitObject.OnNewResult -= onNewResult;
            return true;
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            if (!result.IsHit)
                return;

            switch (judgedObject.HitObject)
            {
                case NoteSheetHead _:
                case NoteSheetTail _:
                    var drawableLanedHit = (IDrawableLanedHit)judgedObject;

                    var star = new DrawableNoteSheetCapStar
                    {
                        Origin = Anchor.Centre,
                        Anchor = drawableLanedHit.LaneAnchor,
                        Size = judgedObject.Size,
                    };

                    var flash = new Circle
                    {
                        Origin = Anchor.Centre,
                        Anchor = drawableLanedHit.LaneAnchor,
                        Size = judgedObject.Size,
                        Scale = new Vector2(0.5f),
                    };

                    star.UpdateColour(drawableLanedHit.LaneAccentColour);
                    flash.Colour = drawableLanedHit.LaneAccentColour.Lighten(0.5f);
                    flash.Alpha = 0.4f;

                    EffectContainer.AddRange(new Drawable[]
                    {
                        star, flash
                    });

                    const float animation_time = 200f;

                    star.ScaleTo(2f, animation_time)
                        .FadeOutFromOne(animation_time)
                        .OnComplete(d => d.Expire());

                    flash.ScaleTo(4f, animation_time / 2)
                         .Then()
                         .ScaleTo(0.5f, animation_time / 2)
                         .FadeOut(animation_time / 2)
                         .OnComplete(d => d.Expire());

                    break;
            }

            if (!judgedObject.DisplayResult || !DisplayJudgements.Value)
                return;

            // TODO: display judgment text etc.
        }
    }
}
