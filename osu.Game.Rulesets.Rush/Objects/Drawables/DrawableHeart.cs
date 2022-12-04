// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public partial class DrawableHeart : DrawableLanedHit<Heart>
    {
        [Resolved]
        private RushPlayfield playfield { get; set; }

        public override bool DisplayResult => false;
        public override bool DisplayExplosion => true;
        protected override float LifetimeEndDelay => 0f;
        protected override bool ExpireOnHit => false;

        public DrawableHeart()
            : this(null)
        {
        }

        public DrawableHeart(Heart hitObject)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE * 2f);

            AddInternal(new ActionBeatSyncedContainer
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.4f),
                NewBeat = (b, t, e, a) => this.ScaleTo(1.5f).Then().ScaleTo(1f, t.BeatLength, Easing.Out),
                Child = new HeartPiece
                {
                    RelativeSizeAxes = Axes.Both,
                }
            });
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged)
                return;

            var result = HitObject.HitWindows.ResultFor(timeOffset);

            if (userTriggered)
            {
                if (result != HitResult.None)
                    ApplyResult(r => r.Type = r.Judgement.MaxResult);

                return;
            }

            // if we haven't passed the hitobject time, do nothing
            if (timeOffset < 0)
                return;

            // if we've passed the object and can longer hit it, miss
            if (result == HitResult.None)
                ApplyResult(r => r.Type = r.Judgement.MinResult);

            // else if we're still able to hit it, check if the player is in the correct lane
            else if (playfield.PlayerSprite.CollidesWith(HitObject))
                ApplyResult(r => r.Type = r.Judgement.MaxResult);
        }
    }
}
