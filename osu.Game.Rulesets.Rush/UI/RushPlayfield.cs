// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osu.Game.Rulesets.Rush.UI.Fever;
using osu.Game.Rulesets.Rush.UI.Ground;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    [Cached]
    public class RushPlayfield : ScrollingPlayfield, IKeyBindingHandler<RushAction>
    {
        public const float DEFAULT_HEIGHT = 178;
        public const float HIT_TARGET_OFFSET = 240;
        public const float HIT_TARGET_SIZE = 100;
        public const float PLAYER_OFFSET = 130;
        public const float JUDGEMENT_OFFSET = 100;
        public const float JUDGEMENT_MOVEMENT = 300;

        public const double HIT_EXPLOSION_DURATION = 200f;

        public RushPlayerSprite PlayerSprite { get; }

        private readonly Container halfPaddingOverEffectContainer;
        internal readonly Container OverPlayerEffectsContainer;

        private readonly LanePlayfield airLane;
        private readonly LanePlayfield groundLane;

        public IEnumerable<DrawableHitObject> AllAliveHitObjects
        {
            get
            {
                if (HitObjectContainer == null)
                    return Enumerable.Empty<DrawableHitObject>();

                // Intentionally counting on the fact that we don't have nested playfields in our nested playfields
                var enumerable = HitObjectContainer.AliveObjects
                                                   .Concat(NestedPlayfields.SelectMany(p => p.HitObjectContainer.AliveObjects))
                                                   .OrderBy(d => d.HitObject.StartTime);
                return enumerable;
            }
        }

        private DrawablePool<DrawableRushJudgement> judgementPool;
        private DrawablePool<DefaultHitExplosion> explosionPool;
        private DrawablePool<StarSheetHitExplosion> sheetExplosionPool;
        private DrawablePool<HeartHitExplosion> heartExplosionPool;
        private DrawablePool<HealthText> healthTextPool;

        [Cached]
        private readonly RushHitPolicy hitPolicy;

        [Cached(type: typeof(IBindable<bool>), name: "feverActivated")]
        private readonly Bindable<bool> feverActivated = new Bindable<bool>();

        public RushPlayfield()
        {
            FeverProcessor feverProcessor;

            hitPolicy = new RushHitPolicy(this);
            RelativeSizeAxes = Axes.X;
            Size = new Vector2(1, DEFAULT_HEIGHT);
            Anchor = Origin = Anchor.Centre;
            InternalChildren = new Drawable[]
            {
                airLane = new LanePlayfield(LanedHitLane.Air),
                groundLane = new LanePlayfield(LanedHitLane.Ground),
                // Contains miniboss and duals for now
                new Container
                {
                    Name = "Hit Objects",
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = HIT_TARGET_OFFSET },
                    Child = HitObjectContainer
                },
                halfPaddingOverEffectContainer = new Container
                {
                    Name = "Over Effects (Half Padding)",
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = HIT_TARGET_OFFSET * 0.75f }
                },
                new GroundDisplay(),
                PlayerSprite = new RushPlayerSprite(),
                OverPlayerEffectsContainer = new Container
                {
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding { Left = PLAYER_OFFSET }
                },
                feverProcessor = new FeverProcessor(),
                new FeverBar(feverProcessor)
            };

            feverActivated.BindTo(feverProcessor.InFeverMode);

            AddNested(airLane);
            AddNested(groundLane);
            NewResult += onNewResult;

            NewResult += (d, j) => feverProcessor.ApplyResult(j);
            RevertResult += (d, j) => feverProcessor.RevertResult(j);
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RegisterPool<MiniBoss, DrawableMiniBoss>(4);
            RegisterPool<MiniBossTick, DrawableMiniBossTick>(10);
            RegisterPool<DualHit, DrawableDualHit>(8);
            RegisterPool<DualHitPart, DrawableDualHitPart>(16);
            RegisterPool<FeverBonus, DrawableFeverBonus>(8);

            AddRangeInternal(new Drawable[]
            {
                judgementPool = new DrawablePool<DrawableRushJudgement>(5),
                explosionPool = new DrawablePool<DefaultHitExplosion>(15),
                sheetExplosionPool = new DrawablePool<StarSheetHitExplosion>(10),
                heartExplosionPool = new DrawablePool<HeartHitExplosion>(2),
                healthTextPool = new DrawablePool<HealthText>(2),
            });
        }

        protected override void OnNewDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            base.OnNewDrawableHitObject(drawableHitObject);

            if (drawableHitObject is DrawableMiniBoss drawableMiniBoss)
                drawableMiniBoss.Attacked += onMiniBossAttacked;

            if (drawableHitObject is DrawableRushHitObject drho)
                drho.CheckHittable = hitPolicy.IsHittable;
        }

        public override void Add(HitObject hitObject)
        {
            switch (hitObject)
            {
                case LanedHit laned:
                    laned.LaneBindable.BindValueChanged(lane =>
                    {
                        if (lane.OldValue != lane.NewValue)
                            playfieldForLane(lane.OldValue).Remove(hitObject);
                        playfieldForLane(lane.OldValue).Add(hitObject);
                    }, true);
                    return;
            }

            base.Add(hitObject);
        }

        private LanePlayfield playfieldForLane(LanedHitLane lane) => lane == LanedHitLane.Air ? airLane : groundLane;

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
                    DrawableStarSheetHead head => sheetExplosionPool.Get(s => s.Apply(head)),
                    DrawableStarSheetTail tail => sheetExplosionPool.Get(s => s.Apply(tail)),
                    DrawableHeart heart => heartExplosionPool.Get(h => h.Apply(heart)),
                    _ => explosionPool.Get(h => h.Apply(rushJudgedObject)),
                };

                if (rushJudgedObject is IDrawableLanedHit laned)
                    playfieldForLane(laned.Lane).AddExplosion(explosion);
            }

            // Display health point difference if the judgement result implies it.
            var pointDifference = rushResult.Judgement.HealthPointIncreaseFor(rushResult);

            if (pointDifference != 0)
                OverPlayerEffectsContainer.Add(healthTextPool.Get(h => h.Apply(pointDifference)));

            // Display judgement results in a drawable for objects that allow it.
            if (rushJudgedObject.DisplayResult)
            {
                DrawableRushJudgement judgementDrawable = judgementPool.Get(j => j.Apply(result, judgedObject));
                LanedHitLane judgementLane = LanedHitLane.Air;

                // TODO: showing judgements based on the judged object suggests that
                //       this may want to be inside the object class as well.
                switch (rushJudgedObject.HitObject)
                {
                    case Sawblade sawblade:
                        judgementLane = sawblade.Lane.Opposite();
                        break;

                    case LanedHit lanedHit:
                        judgementLane = lanedHit.Lane;
                        break;

                    case MiniBoss _:
                        break;
                }

                playfieldForLane(judgementLane).AddJudgement(judgementDrawable);
            }
        }

        public bool OnPressed(RushAction action) => PlayerSprite.HandleAction(action);

        public void OnReleased(RushAction action)
        {
        }
    }
}
