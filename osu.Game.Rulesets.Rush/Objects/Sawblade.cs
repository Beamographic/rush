// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Rush.Scoring;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class Sawblade : LanedHit
    {
        protected override HitWindows CreateHitWindows() => new SawbladeHitWindows();
    }
}
