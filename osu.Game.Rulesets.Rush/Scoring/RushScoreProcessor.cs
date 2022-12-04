// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public partial class RushScoreProcessor : ScoreProcessor
    {
        public RushScoreProcessor(RushRuleset ruleset) : base(ruleset) { }

        protected override JudgementResult CreateResult(HitObject hitObject, Judgement judgement) =>
            new RushJudgementResult((RushHitObject)hitObject, (RushJudgement)judgement);
    }
}
