// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Audio.Track;
using osu.Game.Beatmaps.ControlPoints;
using osu.Game.Graphics.Containers;

namespace osu.Game.Rulesets.Rush.UI
{
    public class ActionBeatSyncedContainer : BeatSyncedContainer
    {
        public Action<int, TimingControlPoint, EffectControlPoint, ChannelAmplitudes> NewBeat;

        protected override void OnNewBeat(int beatIndex, TimingControlPoint timingPoint, EffectControlPoint effectPoint, ChannelAmplitudes amplitudes) =>
            NewBeat?.Invoke(beatIndex, timingPoint, effectPoint, amplitudes);
    }
}
