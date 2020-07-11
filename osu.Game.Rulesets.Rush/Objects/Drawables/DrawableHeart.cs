// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Graphics;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableHeart : DrawableLanedHit<Heart>
    {
        [Resolved]
        private RushPlayfield playfield { get; set; }

        public override bool DisplayResult => false;
        public override bool DisplayExplosion => true;

        public DrawableHeart(Heart hitObject)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE * 2f);

            Content.AddRange(new[]
            {
                new BeatingHeart
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.4f),
                }
            });
        }

        public override Drawable CreateHitExplosion() => new HeartHitExplosion(this);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged)
                return;

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (userTriggered)
            {
                if (result != HitResult.None)
                    ApplyResult(r => r.Type = HitResult.Perfect);

                return;
            }

            // if we haven't passed the hitobject time, do nothing
            if (timeOffset < 0)
                return;

            // if we've passed the object and can longer hit it, miss
            if (result == HitResult.None)
                ApplyResult(r => r.Type = HitResult.Miss);

            // else if we're still able to hit it, check if the player is in the correct lane
            else if (playfield.PlayerSprite.CollidesWith(HitObject))
                ApplyResult(r => r.Type = HitResult.Perfect);
        }

        protected class BeatingHeart : BeatSyncedContainer
        {
            public BeatingHeart()
            {
                Child = new DrawableHeartIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                };
            }

            protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes) =>
                this.ScaleTo(1.5f)
                    .Then()
                    .ScaleTo(1f, timingPoint.BeatLength, Easing.Out);
        }

        private class HeartHitExplosion : DrawableHeartIcon
        {
            public HeartHitExplosion(DrawableHeart drawableHeart)
            {
                Anchor = drawableHeart.LaneAnchor;
                Origin = Anchor.Centre;
                Size = drawableHeart.Size;
                Scale = new Vector2(0.5f);
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                this.ScaleTo(1.25f, RushPlayfield.HIT_EXPLOSION_DURATION)
                    .FadeOutFromOne(RushPlayfield.HIT_EXPLOSION_DURATION)
                    .Expire(true);
            }
        }
    }
}
