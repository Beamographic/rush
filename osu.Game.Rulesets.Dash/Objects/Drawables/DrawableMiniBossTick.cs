// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableMiniBossTick : DrawableDashHitObject<MiniBossTick>
    {
        public override bool DisplayResult => false;

        public DrawableMiniBossTick(MiniBossTick hitObject)
            : base(hitObject)
        {
        }

        protected override void UpdateInitialTransforms() => this.FadeOut();

        public void TriggerResult(HitResult type)
        {
            HitObject.StartTime = Time.Current;
            ApplyResult(r => r.Type = type);
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
        }

        public override bool OnPressed(DashAction action) => false;
    }
}
