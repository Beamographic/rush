// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Rush.Mods;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Scoring
{
    public class RushScoreMultiplierCalculator : ScoreMultiplierCalculator
    {
        public RushScoreMultiplierCalculator(ScoreMultiplierContext context) : base(context)
        {
            #region Difficulty Reduction
            Single<RushModDaycore>(hasMultiplier: 0.3);
            Single<RushModHalfTime>(hasMultiplier: 0.3);
            // No-fail
            #endregion

            #region Difficulty Increase
            Single<RushModDoubleTime>(hasMultiplier: 1.12);
            Single<RushModFlashlight>(hasMultiplier: 1.12);
            Single<RushModNightcore>(hasMultiplier: 1.12);
            // Perfect
            // Sudden death
            #endregion

            #region Automation
            // Autoplay
            // Cinema
            #endregion
        }
    }
}
