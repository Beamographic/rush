// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Dash.Replays
{
    public class DashFramedReplayInputHandler : FramedReplayInputHandler<DashReplayFrame>
    {
        public DashFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }

        protected override bool IsImportant(DashReplayFrame frame) => frame.Actions.Any();

        public override List<IInput> GetPendingInputs()
        {
            return new List<IInput>
            {
                new ReplayState<DashAction>
                {
                    PressedActions = CurrentFrame?.Actions ?? new List<DashAction>(),
                }
            };
        }
    }
}
