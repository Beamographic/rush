// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class LanePlayfield : ScrollingPlayfield
    {
        private readonly LanedHitLane lane;

        public LanePlayfield(LanedHitLane type)
        {
            bool isAirLane = type == LanedHitLane.Air;
            lane = type;

            Name = $"{(isAirLane ? "Air" : "Ground")} Playfield";
            Padding = new MarginPadding { Left = RushPlayfield.HIT_TARGET_OFFSET };
            Anchor = isAirLane ? Anchor.TopLeft : Anchor.BottomLeft;
            Origin = Anchor.CentreLeft;
            RelativeSizeAxes = Axes.Both;
            Size = new Vector2(1, 0.5f);

            AddRangeInternal(new Drawable[]
            {
                new Container
                {
                    Name = "Hit Target",
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.Centre,
                    Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE),
                    Child = new SkinnableDrawable(new RushSkinComponent(isAirLane ? RushSkinComponents.AirHitTarget : RushSkinComponents.GroundHitTarget), _ => new HitTarget
                    {
                        RelativeSizeAxes = Axes.Both,
                    }, confineMode: ConfineMode.ScaleToFit),
                },
                HitObjectContainer,
            });
        }

        [Resolved]
        private RushHitPolicy hitPolicy { get; set; }

        [BackgroundDependencyLoader]
        private void load()
        {
            registerLanedPool<Sawblade, DrawableSawblade>(4);
            registerLanedPool<Heart, DrawableHeart>(2);
            registerLanedPool<StarSheet, DrawableStarSheet>(8);
            registerLanedPool<StarSheetHead, DrawableStarSheetHead>(8);
            registerLanedPool<StarSheetTail, DrawableStarSheetTail>(8);
            registerLanedPool<Minion, DrawableMinion>(8);
            RegisterPool<FeverBonus, DrawableFeverBonus>(8);
        }

        protected override void OnNewDrawableHitObject(DrawableHitObject drawableHitObject)
        {
            base.OnNewDrawableHitObject(drawableHitObject);

            if (drawableHitObject is DrawableRushHitObject drho)
                drho.CheckHittable = hitPolicy.IsHittable;
        }

        private void registerLanedPool<TObject, TDrawable>(int initialSize, int? maximumSize = null) where TObject : LanedHit where TDrawable : DrawableLanedHit<TObject>, new()
        {
            RegisterPool<TObject, TDrawable>(new DrawableLanedObjectPool<TDrawable>(lane, initialSize, maximumSize));
        }

        // This pool pre-initializes created DrawableLanedObjects with a predefined lane value
        // The lane value needs to be set beforehand so that the pieces (Minion, etc) can load using the correct information
        private class DrawableLanedObjectPool<T> : DrawablePool<T> where T : PoolableDrawable, IDrawableLanedHit, new()
        {
            private readonly LanedHitLane lane;

            public DrawableLanedObjectPool(LanedHitLane lane, int initialSize, int? maximumSize)
                : base(initialSize, maximumSize)
            {
                this.lane = lane;
            }

            protected override T CreateNewDrawable() => new T() { Lane = lane };
        }
    }
}
