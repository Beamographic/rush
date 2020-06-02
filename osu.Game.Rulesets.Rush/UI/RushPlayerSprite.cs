// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Textures;
using osu.Framework.Utils;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class RushPlayerSprite : CompositeDrawable
    {
        private const float jump_duration = 300f;
        private const float fall_delay = 500f;
        private const float fall_duration = 300f;
        private const float travel_duration = 150f;

        private readonly TextureAnimation runningAnimation;

        private PlayerTargetLane target;

        public PlayerTargetLane Target
        {
            get => target;
            set
            {
                if (value == target)
                    return;

                target = value;

                switch (value)
                {
                    case PlayerTargetLane.None:
                        fall();
                        break;

                    case PlayerTargetLane.HoldAir:
                    case PlayerTargetLane.AttackAir:
                        easeToAir();
                        break;

                    case PlayerTargetLane.HoldGround:
                    case PlayerTargetLane.AttackGround:
                        easeToGround();
                        break;

                    case PlayerTargetLane.HoldBoth:
                    case PlayerTargetLane.AttackBoth:
                    case PlayerTargetLane.MiniBoss:
                        easeToCentre();
                        break;

                    case PlayerTargetLane.GhostAir:
                        easeToGround();
                        // TODO: show ghost player
                        break;

                    case PlayerTargetLane.GhostGround:
                        easeToAir();
                        // TODO: show ghost player
                        break;
                }

                Target = value & ~PlayerTargetLane.AttackBoth;
            }
        }

        private readonly float groundY;
        private readonly float airY;

        public RushPlayerSprite(float groundY, float airY)
        {
            this.groundY = groundY;
            this.airY = airY;

            InternalChildren = new Drawable[]
            {
                new Circle
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Size = new Vector2(100f)
                }
                // runningAnimation = new TextureAnimation
                // {
                //     Origin = Anchor.Centre,
                //     Anchor = Anchor.Centre,
                //     DefaultFrameLength = 50,
                //     Scale = new Vector2(1)
                // },
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            // runningAnimation.AddFrames(Enumerable.Range(1, 8).Select(i => store.Get($"Player/run_{i}")));
        }

        public void StopAll() => InternalChildren.OfType<TextureAnimation>().ForEach(a =>
        {
            a.Stop();
            a.Hide();
        });

        /// <summary>
        /// Handles any leftover actions that were not consumed by hitobjects.
        /// Allows the player to jump over sawblades or punch the ground.
        /// </summary>
        public bool HandleAction(RushAction action)
        {
            if (Target != PlayerTargetLane.None)
                return false;

            var eq = Precision.AlmostEquals(Y, groundY);

            if ((action == RushAction.AirPrimary || action == RushAction.AirSecondary) && eq)
                jump();
            else if ((action == RushAction.GroundPrimary || action == RushAction.GroundSecondary) && !eq)
                Target |= PlayerTargetLane.AttackGround;

            return true;
        }

        private void jump()
        {
            ClearTransforms();
            this.MoveToY(airY, jump_duration, Easing.Out)
                .Then().Delay(fall_delay)
                .Then().MoveToY(groundY, fall_duration, Easing.In);
        }

        private void fall(bool immediately = false)
        {
            using (BeginDelayedSequence(immediately ? 0 : fall_delay))
                this.MoveToY(groundY, fall_duration, Easing.In);
        }

        private void easeTo(float y)
        {
            ClearTransforms();

            if (Precision.AlmostEquals(Y, y))
                Y = y;
            else
                this.MoveToY(y, travel_duration, Easing.OutQuint);
        }

        private void easeToCentre() => easeTo((airY + groundY) / 2f);

        private void easeToAir() => easeTo(airY);

        private void easeToGround() => easeTo(groundY);

        public void HandleResult(DrawableRushHitObject judgedObject, JudgementResult result)
        {
            switch (judgedObject.HitObject)
            {
                case NoteSheetHead head:
                    Target = Target.WithHoldLane(head.Lane, result.IsHit);
                    break;

                case NoteSheetTail _:
                case NoteSheetBody _:
                    if (judgedObject.HitObject is NoteSheetBody && result.IsHit)
                        break;

                    var lanedHit = (LanedHit)judgedObject.HitObject;

                    Target = Target.WithHoldLane(lanedHit.Lane, false);

                    // special case, need to ensure that we always drop to the ground if there are no holds
                    if ((Target & PlayerTargetLane.HoldBoth) == 0)
                        easeToGround();

                    break;

                case Minion minion when result.IsHit:
                    Target = Target.WithAttackLane(minion.Lane, true);
                    break;

                case DualOrb dualOrb:
                    DrawableDualOrb ddo = (DrawableDualOrb)judgedObject;
                    Target = Target.WithAttackLane(dualOrb.Air.Lane, ddo.Air.Result.IsHit).WithAttackLane(dualOrb.Ground.Lane, ddo.Ground.Result.IsHit);
                    break;

                case MiniBoss _:
                    Target = PlayerTargetLane.None;
                    fall(true);
                    break;
            }
        }
    }

    [Flags]
    public enum PlayerTargetLane
    {
        None = 0,

        HoldAir = 1 << 0,
        HoldGround = 1 << 1,
        HoldBoth = HoldAir | HoldGround,

        AttackAir = 1 << 2,
        AttackGround = 1 << 3,
        AttackBoth = AttackAir | AttackGround,

        GhostAir = HoldGround | AttackAir,
        GhostGround = HoldAir | AttackGround,

        MiniBoss = 1 << 4,
    }

    public static class PlayerTargetLaneExtensions
    {
        public static PlayerTargetLane WithHoldLane(this PlayerTargetLane current, LanedHitLane lane, bool held)
        {
            switch (lane)
            {
                case LanedHitLane.Air:
                    return held ? current | PlayerTargetLane.HoldAir : current & ~PlayerTargetLane.HoldAir;

                case LanedHitLane.Ground:
                    return held ? current | PlayerTargetLane.HoldGround : current & ~PlayerTargetLane.HoldGround;
            }

            return current;
        }

        public static PlayerTargetLane WithAttackLane(this PlayerTargetLane current, LanedHitLane lane, bool attack)
        {
            switch (lane)
            {
                case LanedHitLane.Air:
                    return attack ? current | PlayerTargetLane.AttackAir : current & ~PlayerTargetLane.AttackAir;

                case LanedHitLane.Ground:
                    return attack ? current | PlayerTargetLane.AttackGround : current & ~PlayerTargetLane.AttackGround;
            }

            return current;
        }
    }
}
