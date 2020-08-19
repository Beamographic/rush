// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Skinning;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableSawblade : DrawableLanedHit<Sawblade>
    {
        public Drawable SawPiece => sawContainer.Child.Drawable;

        private readonly Container<SkinnableDrawable> sawContainer;

        [Resolved]
        private RushPlayfield playfield { get; set; }

        // Sawblade uses the reverse lane colour to indicate which key the player should tap to avoid it
        public override Color4 LaneAccentColour => HitObject.Lane == LanedHitLane.Ground ? AIR_ACCENT_COLOUR : GROUND_ACCENT_COLOUR;

        protected override bool ExpireOnHit => false;
        protected override bool ExpireOnMiss => false;

        public DrawableSawblade(Sawblade hitObject)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE * 2f);

            Content.AddRange(new[]
            {
                sawContainer = new Container<SkinnableDrawable>
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.8f),
                    Masking = hitObject.Lane == LanedHitLane.Ground,
                    Child = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.Sawblade, hitObject.Lane), _ => new SawbladePiece())
                    {
                        Origin = Anchor.Centre,
                        Anchor = hitObject.Lane == LanedHitLane.Ground ? Anchor.BottomCentre : Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(0.8f)
                    }
                }
            });
        }

        // Sawblade doesn't handle user presses at all.
        public override bool OnPressed(RushAction action) => false;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (userTriggered || timeOffset < 0 || AllJudged)
                return;

            if (HitObject.HitWindows.CanBeHit(timeOffset))
                return;

            if (playfield.PlayerSprite.CollidesWith(HitObject))
                ApplyResult(r => r.Type = HitResult.Miss);
            else
                ApplyResult(r => r.Type = HitResult.Perfect);
        }
    }
}
