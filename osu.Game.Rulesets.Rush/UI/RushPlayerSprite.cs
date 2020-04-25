// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Rush.Objects;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class RushPlayerSprite : CompositeDrawable, IKeyBindingHandler<RushAction>
    {
        private const float punch_time = 300f;
        private const float travel_time = 150f;

        private readonly TextureAnimation runningAnimation;
        private readonly TextureAnimation fallingAnimation;
        private readonly TextureAnimation[] attackAnimations = new TextureAnimation[2];

        private int attackIndex;

        private readonly float groundY;
        private readonly float airY;

        private bool holdingAir;

        public bool HoldingAir
        {
            get => holdingAir;
            set
            {
                if (holdingAir == value)
                    return;

                holdingAir = value;
                updateHold();
            }
        }

        private bool holdingGround;

        public bool HoldingGround
        {
            get => holdingGround;
            set
            {
                if (holdingGround == value)
                    return;

                holdingGround = value;
                updateHold();
            }
        }

        public RushPlayerSprite(float groundY, float airY)
        {
            this.groundY = groundY;
            this.airY = airY;

            InternalChildren = new Drawable[]
            {
                runningAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    // FillMode = FillMode.Fit,
                    // RelativeSizeAxes = Axes.Both,
                    DefaultFrameLength = 30,
                    Scale = new Vector2(0.5f)
                },
                fallingAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    // FillMode = FillMode.Fit,
                    // RelativeSizeAxes = Axes.Both,
                    DefaultFrameLength = 50,
                    Alpha = 0f,
                    Scale = new Vector2(0.5f)
                },
                attackAnimations[0] = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    // FillMode = FillMode.Fit,
                    // RelativeSizeAxes = Axes.Both,
                    DefaultFrameLength = 50,
                    Alpha = 0f,
                    Scale = new Vector2(0.5f)
                },
                attackAnimations[1] = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    // FillMode = FillMode.Fit,
                    // RelativeSizeAxes = Axes.Both,
                    DefaultFrameLength = 50,
                    Alpha = 0f,
                    Scale = new Vector2(0.5f)
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            runningAnimation.AddFrames(Enumerable.Range(0, 13).Select(i => store.Get($"Player/skeleton-03_run_{i:D2}")));
            fallingAnimation.AddFrames(Enumerable.Range(0, 11).Select(i => store.Get($"Player/skeleton-05_fall_{i:D2}")));
            attackAnimations[0].AddFrames(Enumerable.Range(0, 16).Select(i => store.Get($"Player/skeleton-09_attack_{i:D2}")));
            attackAnimations[1].AddFrames(Enumerable.Range(0, 16).Select(i => store.Get($"Player/skeleton-10_attack_2_{i:D2}")));

            runningAnimation.IsPlaying = true;
            fallingAnimation.IsPlaying = false;
            attackAnimations[0].IsPlaying = false;
            attackAnimations[1].IsPlaying = false;
            attackAnimations[0].Loop = false;
            attackAnimations[1].Loop = false;
        }

        public void StopAll() => InternalChildren.OfType<TextureAnimation>().ForEach(a =>
        {
            a.Stop();
            a.Hide();
        });

        public void PlayRunning()
        {
            StopAll();
            runningAnimation.Show();
            runningAnimation.Play();
        }

        public void PlayFalling()
        {
            StopAll();
            fallingAnimation.Show();
            fallingAnimation.Play();
        }

        public void PlayAttackOnLane(LanedHitLane lane)
        {
            if (holdingAir && lane == LanedHitLane.Ground)
                ; // TODO: ghost punch the ground
            else if (holdingGround && lane == LanedHitLane.Air)
                ; // TODO: ghost punch the air
            else
            {
                StopAll();

                attackAnimations[attackIndex].Show();
                attackAnimations[attackIndex].Play();
                attackIndex = (attackIndex + 1) % attackAnimations.Length;

                var targetY = lane == LanedHitLane.Air ? airY : groundY;
                this.MoveToY(targetY, travel_time, Easing.Out);
            }
        }

        private void updateHold()
        {
            if (holdingAir && holdingGround)
                this.MoveToY(0, travel_time, Easing.OutBack);
            else if (holdingAir)
                this.MoveToY(airY, travel_time, Easing.OutBack);
            else if (holdingGround)
                this.MoveToY(groundY, travel_time, Easing.OutBack);
            else
            {
                PlayFalling();
                this.MoveToY(groundY, travel_time, Easing.Out);
                return;
            }

            // TODO: play hovering animation
        }

        public bool OnPressed(RushAction action)
        {
            // OnPressed/OnReleased will only ever handle actions not
            // caught by hitobjects (this is what we want)

            if (holdingAir || holdingGround)
                return false;

            switch (action)
            {
                default:
                case RushAction.AirPrimary:
                case RushAction.AirSecondary:
                    if (!runningAnimation.IsPlaying)
                        return false;

                    ClearTransforms();

                    this.MoveToY(airY, travel_time, Easing.Out)
                        .Then().Delay(punch_time)
                        .Then().MoveToY(groundY, travel_time, Easing.In)
                        .OnComplete(_ => PlayRunning());

                    // TODO: play jump/fall animations

                    break;

                case RushAction.GroundPrimary:
                case RushAction.GroundSecondary:
                    if (runningAnimation.IsPlaying)
                        return false;

                    ClearTransforms();

                    this.MoveToY(groundY, travel_time, Easing.In)
                        .Then().Delay(punch_time)
                        .Then().MoveToY(groundY)
                        .OnComplete(_ => PlayRunning());

                    // TODO: play punch ground animation

                    break;
            }

            return false;
        }

        public void OnReleased(RushAction action)
        {
        }
    }
}
