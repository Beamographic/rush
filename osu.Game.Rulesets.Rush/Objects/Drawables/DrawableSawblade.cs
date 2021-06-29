// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Rush.Input;
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
        [Resolved]
        private RushPlayfield playfield { get; set; }

        // Sawblade uses the reverse lane colour to indicate which key the player should tap to avoid it
        public override Color4 LaneAccentColour => HitObject.Lane == LanedHitLane.Ground ? AIR_ACCENT_COLOUR : GROUND_ACCENT_COLOUR;

        protected override bool ExpireOnHit => false;
        protected override bool ExpireOnMiss => false;

        private readonly Container sawbladeContainer;
        private readonly Drawable sawblade;

        public DrawableSawblade()
            : this(null)
        {
        }

        public DrawableSawblade(Sawblade hitObject)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE * 2f);

            AddRangeInternal(new[]
            {
                sawbladeContainer = new Container
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.8f),
                    Child = sawblade = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.Sawblade), _ => new SawbladePiece())
                    {
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(0.8f)
                    }
                }
            });
        }

        protected override void OnApply()
        {
            base.OnApply();

            sawbladeContainer.Masking = HitObject.Lane == LanedHitLane.Ground;
            sawblade.Anchor = HitObject.Lane == LanedHitLane.Ground ? Anchor.BottomCentre : Anchor.TopCentre;
        }

        // Sawblade doesn't handle user presses at all.
        public override bool OnPressed(RushAction action) => false;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            // sawblades can't be user triggered, and will not hurt the player in the leading hit windows
            if (userTriggered || timeOffset < 0 || AllJudged)
                return;

            switch (HitObject.HitWindows.ResultFor(timeOffset))
            {
                case HitResult.None:
                    // if we've reached the trailing "none", we successfully dodged the sawblade
                    ApplyResult(r => r.Type = r.Judgement.MaxResult);
                    break;

                case HitResult.Miss:
                    // sawblades only hurt the player if they collide within the trailing "miss" hit window
                    if (playfield.PlayerSprite.CollidesWith(HitObject))
                        ApplyResult(r => r.Type = r.Judgement.MinResult);

                    break;
            }
        }
    }
}
