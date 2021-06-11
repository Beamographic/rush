﻿// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Diagnostics;
using osu.Framework.Extensions.EnumExtensions;
using osu.Game.Beatmaps;
using osu.Game.Replays.Legacy;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Replays.Types;

namespace osu.Game.Rulesets.Rush.Replays
{
    public class RushReplayFrame : ReplayFrame, IConvertibleReplayFrame
    {
        public bool UsingAutoFever { get; private set; }

        public List<RushAction> Actions = new List<RushAction>();

        public RushReplayFrame()
        {
        }

        public RushReplayFrame(double time, RushAction? button = null, bool usingAutoFever = true)
            : base(time)
        {
            UsingAutoFever = usingAutoFever;

            if (button.HasValue)
                Actions.Add(button.Value);
        }

        public RushReplayFrame(double time, IEnumerable<RushAction> buttons, bool usingAutoFever = true)
            : base(time)
        {
            UsingAutoFever = usingAutoFever;

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
            UsingAutoFever = currentFrame.ButtonState.HasFlagFast(ReplayButtonState.Smoke);
        }

        public LegacyReplayFrame ToLegacy(IBeatmap beatmap)
        {
            uint flags = 0;
            foreach (var action in Actions)
                flags |= 1u << (int)action;

            return new LegacyReplayFrame(Time, flags, 0f, UsingAutoFever ? ReplayButtonState.Smoke : ReplayButtonState.None);
        }
    }
}
