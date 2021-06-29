// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Input.StateChanges;
using osu.Game.Replays;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Rush.Configuration;
using osu.Game.Rulesets.Rush.Input;

namespace osu.Game.Rulesets.Rush.Replays
{
    public class RushFramedReplayInputHandler : FramedReplayInputHandler<RushReplayFrame>
    {
        public RushFramedReplayInputHandler(Replay replay)
            : base(replay)
        {
        }

        protected override bool IsImportant(RushReplayFrame frame) => frame.Actions.Any();

        /// <summary>
        /// The current fever activation mode determined by the replay's current frame.
        /// </summary>
        public FeverActivationMode? FeverActivationMode => CurrentFrame?.FeverActivationMode;

        public override void CollectPendingInputs(List<IInput> inputs) =>
            inputs.Add(new ReplayState<RushAction> { PressedActions = CurrentFrame?.Actions ?? new List<RushAction>() });
    }
}
