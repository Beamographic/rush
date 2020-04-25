// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Rush.Replays
{
    public class RushReplayFrame : ReplayFrame
    {
        public List<RushAction> Actions = new List<RushAction>();

        public RushReplayFrame()
        {
        }

        public RushReplayFrame(double time, RushAction? button = null)
            : base(time)
        {
            if (button.HasValue)
                Actions.Add(button.Value);
        }

        public RushReplayFrame(double time, IEnumerable<RushAction> buttons)
            : base(time)
        {
            Actions.AddRange(buttons);
        }
    }
}
