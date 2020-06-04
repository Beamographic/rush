// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class RushScoreProcessor : ScoreProcessor
    {
        public override HitWindows CreateHitWindows() => new RushHitWindows();
    }
}
