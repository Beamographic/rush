// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Rush.Replays
{
    public class RushFramedReplayInputHandler : FramedReplayInputHandler<RushReplayFrame>
    {
        public RushFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }

        protected override bool IsImportant(RushReplayFrame frame) => frame.Actions.Any();

        public override void CollectPendingInputs(List<IInput> inputs) =>
            inputs.Add(new ReplayState<RushAction> { PressedActions = CurrentFrame?.Actions ?? new List<RushAction>() });
    }
}
