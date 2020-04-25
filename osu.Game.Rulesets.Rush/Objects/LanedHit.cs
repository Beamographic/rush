// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class LanedHit : RushHitObject
    {
        public readonly Bindable<LanedHitLane> LaneBindable = new Bindable<LanedHitLane>();

        public virtual LanedHitLane Lane
        {
            get => LaneBindable.Value;
            set => LaneBindable.Value = value;
        }

        public override Judgement CreateJudgement() => new RushJudgement();
    }
}
