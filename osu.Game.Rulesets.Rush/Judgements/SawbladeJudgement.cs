// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Judgements
{
    public class SawbladeJudgement : RushJudgement
    {
        protected override double HealthPointIncreaseFor(HitResult result, bool playerCollided)
        {
            if (playerCollided)
                return -20.0;

            return 0.0;
        }
    }
}
