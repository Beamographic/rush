// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI
{
    [Cached]
    public class RushPlayfield : ScrollingPlayfield, IKeyBindingHandler<RushAction>
    {
        private readonly Random random = new Random();

        public const float DEFAULT_HEIGHT = 178;
        public const float HIT_TARGET_OFFSET = 120;
        public const float HIT_TARGET_SIZE = 100;
        public const float PLAYER_OFFSET = 130;
        public const float JUDGEMENT_OFFSET = 100;
        public const float JUDGEMENT_MOVEMENT = 300;

        public RushPlayerSprite PlayerSprite { get; }

        private readonly Container underEffectContainer;
        private readonly Container overEffectContainer;
        private readonly Container halfPaddingOverEffectContainer;
        private readonly Container overPlayerEffectsContainer;
        private readonly ProxyContainer proxiedHitObjects;
        private readonly JudgementContainer<DrawableRushJudgement> judgementContainer;

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
                        underEffectContainer = new Container
                        {
                            Name = "Under Effects",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET }
                        },
                        judgementContainer = new JudgementContainer<DrawableRushJudgement>
                        {
                            Name = "Judgement",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET }
                        },
                        new Container
                        {
                            Name = "Hit Objects",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                            Child = HitObjectContainer
                        },
                        proxiedHitObjects = new ProxyContainer
                        {
                            Name = "Proxied Hit Objects",
                            RelativeSizeAxes = Axes.Both,
                        },
                        overEffectContainer = new Container
                        {
                            Name = "Over Effects",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET }
                        },
                        halfPaddingOverEffectContainer = new Container
                        {
                            Name = "Over Effects (Half Padding)",
                            RelativeSizeAxes = Axes.Both,
                            Padding = new MarginPadding { Left = HIT_TARGET_OFFSET / 2f }
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
                            overPlayerEffectsContainer = new Container
                            {
                                Origin = Anchor.Centre,
                                Anchor = Anchor.Centre,
                                RelativeSizeAxes = Axes.Both,
                            }
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

            if (hitObject is DrawableMiniBoss drawableMiniBoss)
                drawableMiniBoss.Attacked += onMiniBossAttacked;

            switch (hitObject)
            {
                case DrawableRushHitObject drho:
                    proxiedHitObjects.Add(drho.CreateProxiedContent());
                    break;
            }

            base.Add(hitObject);
        }

        public override bool Remove(DrawableHitObject hitObject)
        {
            if (!base.Remove(hitObject))
                return false;

            hitObject.OnNewResult -= onNewResult;

            if (hitObject is DrawableMiniBoss drawableMiniBoss)
                drawableMiniBoss.Attacked -= onMiniBossAttacked;

            return true;
        }

        private void onMiniBossAttacked(DrawableMiniBoss drawableMiniBoss, double timeOffset)
        {
            var explosion = createHitExplosion(Color4.Yellow.Darken(0.5f), drawableMiniBoss.Anchor);
            explosion.Scale *= 1.5f;
            halfPaddingOverEffectContainer.Add(explosion);
            explosion.ScaleTo(explosion.Scale * 0.5f, 200f).FadeOutFromOne(200f);
            explosion.Delay(200).Expire(true);

            PlayerSprite.Target = PlayerTargetLane.MiniBoss;
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            DrawableRushHitObject rushJudgedObject = (DrawableRushHitObject)judgedObject;
            int healthAmount = (int)((result.Judgement as RushJudgement)?.HealthPoints ?? 0);
            var healthPosition = new Vector2(overPlayerEffectsContainer.DrawWidth * 0.75f, overPlayerEffectsContainer.DrawHeight * 0.5f);

            PlayerSprite.HandleResult(rushJudgedObject, result);

            const float animation_time = 200f;
            const float judgement_time = 250f;

            var drawableLanedHit = rushJudgedObject as IDrawableLanedHit;

            switch (rushJudgedObject.HitObject)
            {
                case NoteSheetHead _ when result.IsHit:
                case NoteSheetTail _ when result.IsHit:
                    var star = new DrawableNoteSheetCapStar
                    {
                        Origin = Anchor.Centre,
                        Anchor = drawableLanedHit!.LaneAnchor,
                        Size = rushJudgedObject.Size,
                    };

                    var flash = new Circle
                    {
                        Origin = Anchor.Centre,
                        Anchor = drawableLanedHit!.LaneAnchor,
                        Size = rushJudgedObject.Size,
                        Scale = new Vector2(0.5f),
                    };

                    star.UpdateColour(drawableLanedHit.LaneAccentColour);
                    flash.Colour = drawableLanedHit.LaneAccentColour.Lighten(0.5f);
                    flash.Alpha = 0.4f;

                    overEffectContainer.AddRange(new Drawable[]
                    {
                        star, flash
                    });

                    star.ScaleTo(2f, animation_time)
                        .FadeOutFromOne(animation_time)
                        .OnComplete(d => d.Expire());

                    flash.ScaleTo(4f, animation_time / 2)
                         .Then()
                         .ScaleTo(0.5f, animation_time / 2)
                         .FadeOut(animation_time / 2)
                         .OnComplete(d => d.Expire());

                    break;

                case Minion _:
                case Orb _:
                    if (result.IsHit)
                    {
                        var explosion = createHitExplosion(drawableLanedHit!.LaneAccentColour, drawableLanedHit.LaneAnchor);
                        underEffectContainer.Add(explosion);
                        explosion.ScaleTo(0.5f, 200f).FadeOutFromOne(200f).OnComplete(d => d.Expire());
                    }
                    else if (PlayerSprite.CollidesWith(result.HitObject))
                    {
                        var damageText = new SpriteText
                        {
                            Origin = Anchor.Centre,
                            Colour = Color4.Red,
                            Font = FontUsage.Default.With(size: 40),
                            Scale = new Vector2(1.2f),
                            Text = $"{healthAmount:D}",
                            Position = healthPosition,
                        };

                        overPlayerEffectsContainer.Add(damageText);

                        damageText.ScaleTo(1f, animation_time)
                                  .Then()
                                  .FadeOutFromOne(animation_time)
                                  .MoveToOffset(new Vector2(0f, -20f), animation_time)
                                  .OnComplete(d => d.Expire());
                    }

                    break;

                case Heart _ when result.IsHit:
                    var heartFlash = new DrawableHeartIcon
                    {
                        Origin = Anchor.Centre,
                        Anchor = drawableLanedHit!.LaneAnchor,
                        Size = rushJudgedObject.Size,
                        Scale = new Vector2(0.5f)
                    };

                    var healthIncrease = new SpriteText
                    {
                        Origin = Anchor.Centre,
                        Colour = Color4.Green,
                        Font = FontUsage.Default.With(size: 40),
                        Scale = new Vector2(1.2f),
                        Text = $"+{healthAmount:D}",
                        Position = healthPosition,
                    };

                    overEffectContainer.Add(heartFlash);

                    heartFlash.ScaleTo(1.25f, animation_time)
                              .FadeOutFromOne(animation_time)
                              .OnComplete(d => d.Expire());

                    overPlayerEffectsContainer.Add(healthIncrease);

                    healthIncrease.ScaleTo(1f, animation_time)
                                  .Then()
                                  .FadeOutFromOne(animation_time)
                                  .MoveToOffset(new Vector2(0f, -20f), animation_time)
                                  .OnComplete(d => d.Expire());

                    // TODO: green floating plus signs

                    break;
            }

            if (!rushJudgedObject.DisplayResult || !DisplayJudgements.Value)
                return;

            DrawableRushJudgement judgementDrawable = null;

            switch (rushJudgedObject.HitObject)
            {
                case NoteSheet _:
                    break;

                case Heart _:
                    break;

                case Sawblade sawblade:
                    judgementDrawable = new DrawableRushJudgement(result, rushJudgedObject)
                    {
                        Origin = Anchor.Centre,
                        Position = new Vector2(0f, judgementPositionForLane(sawblade.Lane.Opposite())),
                        Scale = new Vector2(1.2f)
                    };

                    break;

                case LanedHit lanedHit:
                    judgementDrawable = new DrawableRushJudgement(result, rushJudgedObject)
                    {
                        Origin = Anchor.Centre,
                        Position = new Vector2(0f, judgementPositionForLane(lanedHit.Lane)),
                        Scale = new Vector2(1.5f)
                    };

                    break;
            }

            if (judgementDrawable != null)
            {
                judgementContainer.Add(judgementDrawable);

                judgementDrawable.ScaleTo(1f, judgement_time)
                                 .Then()
                                 .MoveToOffset(new Vector2(-JUDGEMENT_MOVEMENT, 0f), judgement_time, Easing.In)
                                 .OnComplete(d => d.Expire());
            }
        }

        private float judgementPositionForLane(LanedHitLane lane) => lane == LanedHitLane.Air ? -JUDGEMENT_OFFSET : judgementContainer.DrawHeight - JUDGEMENT_OFFSET;

        private Drawable createHitExplosion(Color4 colour, Anchor anchor = Anchor.Centre)
        {
            // some random rotation and scale for variety
            var startScale = 0.9f + random.NextDouble() * 0.2f;
            var rotation = random.NextDouble() * 360;

            return new DefaultHitExplosion(colour)
            {
                Origin = Anchor.Centre,
                Anchor = anchor,
                Size = new Vector2(200, 200),
                Scale = new Vector2((float)startScale),
                Rotation = (float)rotation
            };
        }

        public bool OnPressed(RushAction action) => PlayerSprite.HandleAction(action);

        public void OnReleased(RushAction action)
        {
        }

        private class ProxyContainer : LifetimeManagementContainer
        {
            public new MarginPadding Padding
            {
                set => base.Padding = value;
            }

            public void Add(Drawable proxy) => AddInternal(proxy);
        }
    }
}
