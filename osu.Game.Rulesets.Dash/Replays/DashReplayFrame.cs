// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Dash.Replays
{
    public class DashReplayFrame : ReplayFrame
    {
        public List<DashAction> Actions = new List<DashAction>();

        public DashReplayFrame()
        {
        }

        public DashReplayFrame(double time, DashAction? button = null)
            : base(time)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }

        public DashReplayFrame(double time, IEnumerable<DashAction> buttons)
            : base(time)
        {
            Actions.AddRange(buttons);
        }
    }
}
