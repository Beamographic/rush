// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Dash.Objects;
using osu.Game.Rulesets.Dash.Objects.Drawables;
using osu.Game.Rulesets.Dash.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;

namespace osu.Game.Rulesets.Dash.UI
{
    [Cached]
    public class DrawableDashRuleset : DrawableScrollingRuleset<DashHitObject>
    {
        protected override bool UserScrollSpeedAdjustment => false;

        protected override ScrollVisualisationMethod VisualisationMethod => ScrollVisualisationMethod.Constant;

        public DrawableDashRuleset(DashRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
            Direction.Value = ScrollingDirection.Left;
            TimeRange.Value = 750;
        }

        protected override Playfield CreatePlayfield() => new DashPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new DashFramedReplayInputHandler(replay);

        public override DrawableHitObject<DashHitObject> CreateDrawableRepresentation(DashHitObject h)
        {
            switch (h)
            {
                case Minion minion:
                    return new DrawableMinion(minion);

                case MiniBoss miniBoss:
                    return new DrawableMiniBoss(miniBoss);

                case NoteSheet noteSheet:
                    return new DrawableNoteSheet(noteSheet);

                case DualOrb dualOrb:
                    return new DrawableDualOrb(dualOrb);
            }

            return null;
        }

        protected override PassThroughInputManager CreateInputManager() => new DashInputManager(Ruleset?.RulesetInfo);
    }
}
