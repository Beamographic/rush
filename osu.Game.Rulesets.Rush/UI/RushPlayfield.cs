// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Utils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osu.Game.Rulesets.Rush.UI.Ground;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI
{
    [Cached]
    public class RushPlayfield : ScrollingPlayfield, IKeyBindingHandler<RushAction>
    {
        public const float DEFAULT_HEIGHT = 178;
        public const float HIT_TARGET_OFFSET = 120;
        public const float HIT_TARGET_SIZE = 100;
        public const float PLAYER_OFFSET = 130;
        public const float JUDGEMENT_OFFSET = 100;
        public const float JUDGEMENT_MOVEMENT = 300;

        public const double HIT_EXPLOSION_DURATION = 200f;

        public RushPlayerSprite PlayerSprite { get; }

        private readonly Container underEffectContainer;
        private readonly Container overEffectContainer;
        private readonly Container halfPaddingOverEffectContainer;
        private readonly Container overPlayerEffectsContainer;
        private readonly ProxyContainer proxiedHitObjects;
        private readonly JudgementContainer<DrawableJudgement> judgementContainer;

        private DrawablePool<DrawableRushJudgement> judgementPool;
        private DrawablePool<DefaultHitExplosion> explosionPool;
        private DrawablePool<HeartHitExplosion> heartsplosionPool;
        private DrawablePool<StarSheetHitExplosion> sheetsplosionPool;
        private DrawablePool<HealthText> healthTextPool;

        public RushPlayfield()
        {
            InternalChildren = new Drawable[]
            {
                new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RowDimensions = new[]
                    {
                        new Dimension(GridSizeMode.Distributed), // Top empty area
                        new Dimension(GridSizeMode.Absolute, DEFAULT_HEIGHT), // Playfield area
                        new Dimension(GridSizeMode.Distributed), // Ground area, extends to overall height
                    },
                    Content = new[]
                    {
                        new[]
                        {
                            Empty()
                        },
                        new Drawable[]
                        {
                            new Container
                            {
                                Name = "Playfield area",
                                RelativeSizeAxes = Axes.Both,
                                Children = new[]
                                {
                                    new Container
                                    {
                                        Name = "Left area",
                                        Width = HIT_TARGET_OFFSET,
                                        RelativeSizeAxes = Axes.Y,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Depth = -1,
                                        Child = new Container
                                        {
                                            Name = "Left Play Zone",
                                            RelativeSizeAxes = Axes.Both,
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
                                        },
                                    },
                                    new Container
                                    {
                                        Name = "Right area",
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Children = new Drawable[]
                                        {
                                            new Container
                                            {
                                                Name = "Hit target indicators",
                                                RelativeSizeAxes = Axes.Both,
                                                Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                                                Children = new Drawable[]
                                                {
                                                    new Container
                                                    {
                                                        Anchor = Anchor.TopLeft,
                                                        Origin = Anchor.Centre,
                                                        Size = new Vector2(HIT_TARGET_SIZE),
                                                        Child = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.AirHitTarget), _ => new HitTarget()
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                        }, confineMode: ConfineMode.ScaleToFit),
                                                    },
                                                    new Container
                                                    {
                                                        Anchor = Anchor.BottomLeft,
                                                        Origin = Anchor.Centre,
                                                        Size = new Vector2(HIT_TARGET_SIZE),
                                                        Child = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.GroundHitTarget), _ => new HitTarget()
                                                        {
                                                            RelativeSizeAxes = Axes.Both,
                                                        }, confineMode: ConfineMode.ScaleToFit),
                                                    },
                                                }
                                            },
                                            underEffectContainer = new Container
                                            {
                                                Name = "Under Effects",
                                                RelativeSizeAxes = Axes.Both,
                                                Padding = new MarginPadding { Left = HIT_TARGET_OFFSET }
                                            },
                                            judgementContainer = new JudgementContainer<DrawableJudgement>
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
                                    }
                                }
                            },
                        },
                        new Drawable[]
                        {
                            new Container
                            {
                                Name = "Ground area",
                                RelativeSizeAxes = Axes.Both,
                                // Due to the size of the player sprite, we have to push the ground even more to the bottom.
                                Padding = new MarginPadding { Top = 50f },
                                Depth = float.MaxValue,
                                Child = new GroundDisplay(),
                            }
                        }
                    }
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            NewResult += onNewResult;

            RegisterPool<Minion, DrawableMinion>(8);
            RegisterPool<MiniBoss, DrawableMiniBoss>(4);
            RegisterPool<MiniBossTick, DrawableMiniBossTick>(10);
            RegisterPool<StarSheet, DrawableStarSheet>(8);
            RegisterPool<StarSheetHead, DrawableStarSheetHead>(8);
            RegisterPool<StarSheetTail, DrawableStarSheetTail>(8);
            RegisterPool<DualHit, DrawableDualHit>(8);
            RegisterPool<DualHitPart, DrawableDualHitPart>(16);
            RegisterPool<Sawblade, DrawableSawblade>(4);
            RegisterPool<Heart, DrawableHeart>(2);

            AddRangeInternal(new Drawable[]{
                judgementPool = new DrawablePool<DrawableRushJudgement>(5),
                explosionPool = new DrawablePool<DefaultHitExplosion>(5),
                heartsplosionPool = new DrawablePool<HeartHitExplosion>(5),
                sheetsplosionPool = new DrawablePool<StarSheetHitExplosion>(5),
                healthTextPool = new DrawablePool<HealthText>(2),
            });
        }
        protected override void OnNewDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            base.OnNewDrawableHitObject(drawableHitObject);

            if (drawableHitObject is DrawableMiniBoss drawableMiniBoss)
                drawableMiniBoss.Attacked += onMiniBossAttacked;

            switch (drawableHitObject)
            {
                case DrawableRushHitObject drho:
                    proxiedHitObjects.Add(drho.CreateProxiedContent());
                    break;
            }
        }

        protected override void OnHitObjectAdded(HitObject hitObject)
        {
            base.OnHitObjectAdded(hitObject);
        }


        public override void Add(DrawableHitObject hitObject)
        {
            base.Add(hitObject);
        }

        public override bool Remove(DrawableHitObject hitObject)
        {
            if (!base.Remove(hitObject))
                return false;

            if (hitObject is DrawableMiniBoss drawableMiniBoss)
                drawableMiniBoss.Attacked -= onMiniBossAttacked;

            return true;
        }

        private void onMiniBossAttacked(DrawableMiniBoss drawableMiniBoss, double timeOffset)
        {

            halfPaddingOverEffectContainer.Add(explosionPool.Get(h => h.Apply(drawableMiniBoss)));

            PlayerSprite.Target = PlayerTargetLane.MiniBoss;
        }

        private void onNewResult(DrawableHitObject judgedObject, JudgementResult result)
        {
            DrawableRushHitObject rushJudgedObject = (DrawableRushHitObject)judgedObject;
            RushJudgementResult rushResult = (RushJudgementResult)result;

            PlayerSprite.HandleResult(rushJudgedObject, result);

            // Display hit explosions for objects that allow it.
            if (result.IsHit && rushJudgedObject.DisplayExplosion)
            {
                Drawable explosion = rushJudgedObject switch
                {
                    DrawableStarSheetHead head => sheetsplosionPool.Get(s => s.Apply(head)),
                    DrawableStarSheetTail tail => sheetsplosionPool.Get(s => s.Apply(tail)),
                    DrawableHeart heart => heartsplosionPool.Get(h => h.Apply(heart)),
                    _ => explosionPool.Get(h => h.Apply(rushJudgedObject)),
                };

                if (explosion != null)
                {
                    // TODO: low priority, but the explosion should be added in a container
                    //       that has the hit object container to avoid this kinda hacky check.
                    if (explosion.Depth <= 0)
                        overEffectContainer.Add(explosion);
                    else
                        underEffectContainer.Add(explosion);
                }
            }

            // Display health point difference if the judgement result implies it.
            var pointDifference = rushResult.Judgement.HealthPointIncreaseFor(rushResult);

            if (pointDifference != 0)
                overPlayerEffectsContainer.Add(healthTextPool.Get(h => h.Apply(pointDifference)));

            // Display judgement results in a drawable for objects that allow it.
            if (rushJudgedObject.DisplayResult)
            {
                DrawableRushJudgement judgementDrawable = judgementPool.Get(j => j.Apply(result, judgedObject));

                // TODO: showing judgements based on the judged object suggests that
                //       this may want to be inside the object class as well.
                switch (rushJudgedObject.HitObject)
                {
                    case Sawblade sawblade:
                        judgementDrawable.StartPosition = new Vector2(0f, judgementPositionForLane(sawblade.Lane.Opposite()));
                        judgementDrawable.StartScale = new Vector2(1.2f);

                        break;

                    case LanedHit lanedHit:
                        judgementDrawable.StartPosition = new Vector2(0f, judgementPositionForLane(lanedHit.Lane));
                        judgementDrawable.StartScale = new Vector2(1.5f);

                        break;
                }

                judgementContainer.Add(judgementDrawable);
            }
        }

        private float judgementPositionForLane(LanedHitLane lane) => lane == LanedHitLane.Air ? -JUDGEMENT_OFFSET : judgementContainer.DrawHeight - JUDGEMENT_OFFSET;

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
