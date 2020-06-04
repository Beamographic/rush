// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableMiniBoss : DrawableRushHitObject<MiniBoss>
    {
        private const float base_sprite_scale = 1f;
        private const float target_sprite_scale = 1.1f;
        private readonly Vector2 spriteSize = new Vector2(300f, 200f);

        private readonly TextureAnimation normalAnimation;
        private readonly TextureAnimation hitAnimation;

        private readonly Container<DrawableMiniBossTick> ticks;

        public event Action<DrawableMiniBoss, double> Attacked;

        public DrawableMiniBoss(MiniBoss hitObject)
            : base(hitObject)
        {
            Origin = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                normalAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Scale = new Vector2(base_sprite_scale),
                    DefaultFrameLength = 250,
                },
                hitAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Scale = new Vector2(base_sprite_scale),
                    DefaultFrameLength = 250,
                    Alpha = 0f
                },
                ticks = new Container<DrawableMiniBossTick> { RelativeSizeAxes = Axes.Both }
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            normalAnimation.AddFrames(new[] { store.Get("MiniBoss/pippidon_boss_0"), store.Get("MiniBoss/pippidon_boss_1") });
            hitAnimation.AddFrames(new[] { store.Get("MiniBoss/pippidon_boss_hurt_0"), store.Get("MiniBoss/pippidon_boss_hurt_1"), store.Get("MiniBoss/pippidon_boss_hurt_2") });
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableMiniBossTick tick:
                    ticks.Add(tick);
                    break;
            }
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            ticks.Clear();
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case MiniBossTick tick:
                    return new DrawableMiniBossTick(tick);
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void Update()
        {
            base.Update();

            float fraction = (float)(HitObject.StartTime - Clock.CurrentTime) / 500f;
            normalAnimation.Y = (float)(Math.Sin(fraction * 2 * Math.PI) * 5f);

            X = Math.Max(0, X);
        }

        protected override void UpdateInitialTransforms()
        {
            normalAnimation.Show();
            hitAnimation.Hide();
        }

        public override bool OnPressed(RushAction action) => UpdateResult(true);

        public override void OnReleased(RushAction action)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged)
                return;

            if (userTriggered && timeOffset < 0)
            {
                DrawableMiniBossTick nextTick = null;

                foreach (var t in ticks)
                {
                    if (!t.IsHit)
                    {
                        nextTick = t;
                        break;
                    }
                }

                nextTick?.TriggerResult(HitResult.Perfect);

                var numHits = ticks.Count(r => r.IsHit);
                var completion = (float)numHits / HitObject.RequiredHits;

                normalAnimation.Hide();
                hitAnimation.Show();
                hitAnimation.ScaleTo(base_sprite_scale + Math.Min(target_sprite_scale - base_sprite_scale, (target_sprite_scale - base_sprite_scale) * completion), 260, Easing.OutQuint);

                OnAttacked(this, timeOffset);
            }
            else if (!userTriggered && timeOffset >= 0)
            {
                int numHits = 0;

                foreach (var tick in ticks)
                {
                    if (tick.IsHit)
                    {
                        numHits++;
                        continue;
                    }

                    tick.TriggerResult(HitResult.Miss);
                }

                var hitResult = numHits == HitObject.RequiredHits
                    ? HitResult.Great
                    : numHits > HitObject.RequiredHits / 2
                        ? HitResult.Good
                        : HitResult.Miss;

                ApplyResult(r => r.Type = hitResult);
            }
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Idle:
                    UnproxyContent();
                    break;

                case ArmedState.Miss:
                    this.FadeOut(300);
                    break;

                case ArmedState.Hit:
                    ProxyContent();

                    const float gravity_time = 300;
                    const float gravity_travel_height = 500f;

                    using (BeginAbsoluteSequence(Time.Current, true))
                    {
                        this.RotateTo(-180, gravity_time);
                        this.MoveToY(-gravity_travel_height, gravity_time, Easing.Out);
                        this.FadeOut(gravity_time);
                    }

                    break;
            }
        }

        protected virtual void OnAttacked(DrawableMiniBoss drawableMiniBoss, double timeOffset) => Attacked?.Invoke(drawableMiniBoss, timeOffset);
    }
}
