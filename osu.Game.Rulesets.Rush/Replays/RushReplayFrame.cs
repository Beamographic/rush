// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;

namespace osu.Game.Rulesets.Rush.Replays
{
    public class RushReplayFrame : ReplayFrame, IConvertibleReplayFrame
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

        public void FromLegacy(LegacyReplayFrame currentFrame, IBeatmap beatmap, ReplayFrame lastFrame = null)
        {
            if (currentFrame.MouseLeft1) Actions.Add(RushAction.AirPrimary);
            if (currentFrame.MouseLeft2) Actions.Add(RushAction.AirSecondary);
            if (currentFrame.MouseRight1) Actions.Add(RushAction.GroundPrimary);
            if (currentFrame.MouseRight2) Actions.Add(RushAction.GroundSecondary);
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            ReplayButtonState state = ReplayButtonState.None;

            if (Actions.Contains(RushAction.AirPrimary) || Actions.Contains(RushAction.AirTertiary))
                state |= ReplayButtonState.Left1;

            if (Actions.Contains(RushAction.AirSecondary) || Actions.Contains(RushAction.AirQuaternary))
                state |= ReplayButtonState.Left2;

            if (Actions.Contains(RushAction.GroundPrimary) || Actions.Contains(RushAction.GroundSecondary))
                state |= ReplayButtonState.Right1;

            if (Actions.Contains(RushAction.GroundTertiary) || Actions.Contains(RushAction.GroundQuaternary))
                state |= ReplayButtonState.Right2;

            return new LegacyReplayFrame(Time, 0f, 0f, state);
        }
    }
}
