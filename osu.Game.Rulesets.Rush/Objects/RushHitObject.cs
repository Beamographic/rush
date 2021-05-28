// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Scoring;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class RushHitObject : HitObject
    {
        public override Judgement CreateJudgement() => new RushJudgement();

        protected override HitWindows CreateHitWindows() => new RushHitWindows();

        protected virtual bool HasFeverBonus => true;

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            if (HasFeverBonus)
                AddNested(new FeverBonus() { StartTime = this.GetEndTime() });
        }
    }
}
