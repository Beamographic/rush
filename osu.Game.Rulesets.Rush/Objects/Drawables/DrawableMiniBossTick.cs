// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableMiniBossTick : DrawableRushHitObject<MiniBossTick>
    {
        public override bool DisplayResult => false;

        public DrawableMiniBossTick()
            : this(null)
        {
        }

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

        public override bool OnPressed(RushAction action) => false;
    }
}
