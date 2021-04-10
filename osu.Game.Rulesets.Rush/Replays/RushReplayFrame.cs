// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Debug.Assert(currentFrame.MouseX != null);
            Actions.AddRange(getActionsFromFlags((RushActionFlags)currentFrame.MouseX));
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            return new LegacyReplayFrame(Time, (float)getFlagsFromActions(Actions), 0f, ReplayButtonState.None);
        }

        private static RushActionFlags getFlagsFromActions(IEnumerable<RushAction> actions)
        {
            RushActionFlags flags = RushActionFlags.None;

            if (actions.Contains(RushAction.AirPrimary)) flags |= RushActionFlags.AirPrimary;
            if (actions.Contains(RushAction.AirSecondary)) flags |= RushActionFlags.AirSecondary;
            if (actions.Contains(RushAction.AirTertiary)) flags |= RushActionFlags.AirTertiary;
            if (actions.Contains(RushAction.AirQuaternary)) flags |= RushActionFlags.AirQuaternary;
            if (actions.Contains(RushAction.GroundPrimary)) flags |= RushActionFlags.GroundPrimary;
            if (actions.Contains(RushAction.GroundSecondary)) flags |= RushActionFlags.GroundSecondary;
            if (actions.Contains(RushAction.GroundTertiary)) flags |= RushActionFlags.GroundTertiary;
            if (actions.Contains(RushAction.GroundQuaternary)) flags |= RushActionFlags.GroundQuaternary;

            return flags;
        }

        private static IEnumerable<RushAction> getActionsFromFlags(RushActionFlags flags)
        {
            if (flags.HasFlagFast(RushActionFlags.AirPrimary)) yield return RushAction.AirPrimary;
            if (flags.HasFlagFast(RushActionFlags.AirSecondary)) yield return RushAction.AirSecondary;
            if (flags.HasFlagFast(RushActionFlags.AirTertiary)) yield return RushAction.AirTertiary;
            if (flags.HasFlagFast(RushActionFlags.AirQuaternary)) yield return RushAction.AirQuaternary;
            if (flags.HasFlagFast(RushActionFlags.GroundPrimary)) yield return RushAction.GroundPrimary;
            if (flags.HasFlagFast(RushActionFlags.GroundSecondary)) yield return RushAction.GroundSecondary;
            if (flags.HasFlagFast(RushActionFlags.GroundTertiary)) yield return RushAction.GroundTertiary;
            if (flags.HasFlagFast(RushActionFlags.GroundQuaternary)) yield return RushAction.GroundQuaternary;
        }

        [Flags]
        private enum RushActionFlags
        {
            None = 0,
            AirPrimary = 1 << 0,
            AirSecondary = 1 << 1,
            AirTertiary = 1 << 2,
            AirQuaternary = 1 << 3,
            GroundPrimary = 1 << 4,
            GroundSecondary = 1 << 5,
            GroundTertiary = 1 << 6,
            GroundQuaternary = 1 << 7,
        }
    }
}
