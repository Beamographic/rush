// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Dash.Judgements;
using osu.Game.Rulesets.Judgements;

namespace osu.Game.Rulesets.Dash.Objects
{
    public class NoteSheetTail : LanedHit
    {
        public override Judgement CreateJudgement() => new DashJudgement();
    }
}
