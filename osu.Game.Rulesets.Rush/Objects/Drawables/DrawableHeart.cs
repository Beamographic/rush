// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableHeart : DrawableLanedHit<Heart>
    {
        [Resolved]
        private RushPlayfield playfield { get; set; }

        public override bool DisplayResult => false;
        public override bool DisplayExplosion => true;

        public DrawableHeart()
            : this(null)
        {
        }

        public DrawableHeart([CanBeNull] Heart hitObject = null)
            : base(hitObject)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE * 2f);

            Content.Add(new ActionBeatSyncedContainer
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
                Size = new Vector2(0.4f),
                NewBeat = (b, t, e, a) => this.ScaleTo(1.5f).Then().ScaleTo(1f, t.BeatLength, Easing.Out),
                Child = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.Heart), _ => new HeartPiece
                {
                    RelativeSizeAxes = Axes.Both,
                })
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

        private class HeartHitExplosion : HeartPiece
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
