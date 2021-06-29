// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Rush.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class RushReplayRecorder : ReplayRecorder<RushAction>
    {
        [Resolved(canBeNull: true)]
        private DrawableRushRuleset drawableRuleset { get; set; }

        public RushReplayRecorder(Score target)
            : base(target)
        {
        }

        protected override ReplayFrame HandleFrame(Vector2 mousePosition, List<RushAction> actions, ReplayFrame previousFrame)
        {
            var frame = new RushReplayFrame(Time.Current, actions);

            if (drawableRuleset != null)
                frame.FeverActivationMode = drawableRuleset.FeverActivationMode;

            return frame;
        }
    }
}
