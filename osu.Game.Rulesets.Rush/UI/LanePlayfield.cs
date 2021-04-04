// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class LanePlayfield : ScrollingPlayfield
    {
        private readonly JudgementContainer<DrawableJudgement> judgementContainer;
        private readonly Container effectsContainer;
        public LanePlayfield(LanedHitLane type)
        {
            bool isAirLane = type == LanedHitLane.Air;

            Padding = new MarginPadding { Left = RushPlayfield.HIT_TARGET_OFFSET };
            Anchor = Origin = isAirLane ? Anchor.TopCentre : Anchor.BottomCentre;
            RelativeSizeAxes = Axes.Both;
            Size = new Vector2(1, 0);

            AddRangeInternal(new Drawable[]{
                new Container
                {
                    Name = "Hit Target",
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.Centre,
                    Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE),
                    Child = new SkinnableDrawable(new RushSkinComponent(isAirLane ? RushSkinComponents.AirHitTarget : RushSkinComponents.GroundHitTarget), _ => new HitTarget()
                    {
                        RelativeSizeAxes = Axes.Both,
                    }, confineMode: ConfineMode.ScaleToFit),
                },
                effectsContainer = new Container(),
                judgementContainer = new JudgementContainer<DrawableJudgement>(),
                HitObjectContainer,
            });
            NewResult += onNewResult;
        }

        private void onNewResult(DrawableHitObject hitObject, JudgementResult result)
        {
            var RushHitObject = (DrawableRushHitObject)hitObject;

            // Display hit explosions for objects that allow it.
            if (result.IsHit && RushHitObject.DisplayExplosion)
            {
                effectsContainer.Add(RushHitObject.CreateHitExplosion());
            }

            if (hitObject.DisplayResult)
            {
                judgementContainer.Add(new DrawableRushJudgement(result, RushHitObject)
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Position = new Vector2(0f, -80f),
                    Scale = new Vector2(1.5f)
                });
            }
        }
    }
}
