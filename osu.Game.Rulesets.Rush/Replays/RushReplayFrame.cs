// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Extensions.EnumExtensions;
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

            if (currentFrame.MouseX != null)
                Actions.AddRange(getActionsFromFlags((RushSideActionFlags)currentFrame.MouseX));
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            ReplayButtonState state = ReplayButtonState.None;

            if (Actions.Contains(RushAction.AirPrimary)) state |= ReplayButtonState.Left1;
            if (Actions.Contains(RushAction.AirSecondary)) state |= ReplayButtonState.Left2;
            if (Actions.Contains(RushAction.GroundPrimary)) state |= ReplayButtonState.Right1;
            if (Actions.Contains(RushAction.GroundSecondary)) state |= ReplayButtonState.Right2;

            return new LegacyReplayFrame(Time, (float)getFlagsFromActions(Actions), 0f, state);
        }

        private static RushSideActionFlags getFlagsFromActions(IEnumerable<RushAction> actions)
        {
            RushSideActionFlags flags = RushSideActionFlags.None;

            if (actions.Contains(RushAction.AirTertiary)) flags |= RushSideActionFlags.AirTertiary;
            if (actions.Contains(RushAction.AirQuaternary)) flags |= RushSideActionFlags.AirQuaternary;
            if (actions.Contains(RushAction.GroundTertiary)) flags |= RushSideActionFlags.GroundTertiary;
            if (actions.Contains(RushAction.GroundQuaternary)) flags |= RushSideActionFlags.GroundQuaternary;

            return flags;
        }

        private static IEnumerable<RushAction> getActionsFromFlags(RushSideActionFlags sideFlags)
        {
            if (sideFlags.HasFlagFast(RushSideActionFlags.AirTertiary)) yield return RushAction.AirTertiary;
            if (sideFlags.HasFlagFast(RushSideActionFlags.AirQuaternary)) yield return RushAction.AirQuaternary;
            if (sideFlags.HasFlagFast(RushSideActionFlags.GroundTertiary)) yield return RushAction.GroundTertiary;
            if (sideFlags.HasFlagFast(RushSideActionFlags.GroundQuaternary)) yield return RushAction.GroundQuaternary;
        }

        [Flags]
        private enum RushSideActionFlags
        {
            None = 0,
            AirTertiary = 1 << 2,
            AirQuaternary = 1 << 3,
            GroundTertiary = 1 << 6,
            GroundQuaternary = 1 << 7,
        }
    }
}
