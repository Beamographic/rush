// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Diagnostics;
using osu.Framework.Extensions.EnumExtensions;
using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Rush.Configuration;
using osu.Game.Rulesets.Rush.Input;

namespace osu.Game.Rulesets.Rush.Replays
{
    public class RushReplayFrame : ReplayFrame, IConvertibleReplayFrame
    {
        public List<RushAction> Actions = new List<RushAction>();

        /// <summary>
        /// The fever activation mode at this frame.
        /// </summary>
        public FeverActivationMode FeverActivationMode;

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

            uint flags = (uint)currentFrame.MouseX;

            int currentBit = 0;

            while (flags > 0)
            {
                if ((flags & 1) > 0)
                    Actions.Add((RushAction)currentBit);

                ++currentBit;
                flags >>= 1;
            }

            // We are repurposing ReplayButtonState.Smoke in order to store the AutoFever setting used at the time of recording.
            // This will serve as an interim solution until non-legacy replays are supported in osu.
            FeverActivationMode = getFeverActivationMode(currentFrame.ButtonState);
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            uint flags = 0;
            foreach (var action in Actions)
                flags |= 1u << (int)action;

            return new LegacyReplayFrame(Time, flags, 0f, getFeverActivationButtonState(FeverActivationMode));
        }

        private static FeverActivationMode getFeverActivationMode(ReplayButtonState buttonState) => buttonState.HasFlagFast(ReplayButtonState.Smoke) switch
        {
            true => FeverActivationMode.Automatic,
            false => FeverActivationMode.Manual
        };

        private static ReplayButtonState getFeverActivationButtonState(FeverActivationMode mode) => mode switch
        {
            FeverActivationMode.Automatic => ReplayButtonState.Smoke,
            _ => ReplayButtonState.None,
        };
    }
}
