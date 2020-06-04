// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Rush.Scoring;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class Heart : LanedHit
    {
        public override Judgement CreateJudgement() => new HeartJudgement();

        protected override HitWindows CreateHitWindows() => new HeartHitWindows();
    }
}
