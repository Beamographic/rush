// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Rush.Judgements;

namespace osu.Game.Rulesets.Rush.Objects
{
    /// <summary>
    /// Uses a sprite based on the beatmap environment and note hitsound.
    /// </summary>
    public class Minion : LanedHit
    {
        public override Judgement CreateJudgement() => new CollisionDamagingJudgement();
    }
}
