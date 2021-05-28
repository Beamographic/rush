// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Objects.Drawables;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class FeverBonus : RushHitObject
    {
        public override Judgement CreateJudgement() => new RushFeverJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;

        protected override bool HasFeverBonus => false;
    }
}
