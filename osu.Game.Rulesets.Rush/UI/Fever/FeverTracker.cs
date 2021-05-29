// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Rush.Judgements;

namespace osu.Game.Rulesets.Rush.UI.Fever
{
    public class FeverTracker : Component, IKeyBindingHandler<RushAction>
    {
        private const float fever_duration = 10000;
        public Bindable<bool> FeverActivated = new Bindable<bool>();
        public Bindable<float> FeverProgress = new Bindable<float>();

        private List<FeverEvent> eventsTimeline = new List<FeverEvent>{
            new FeverEvent(double.MinValue, 0, FeverEvent.EventType.SessionBegin)
        };

        private const int perfect_hits_to_fill = 50;

        protected override void Update()
        {
            base.Update();
            if (Clock.Rate < 0)
            {
                // Reverse handling stuff
                int removeStartIndex = 0;
                for (int i = 0; i < eventsTimeline.Count; ++i)
                {
                    if (eventsTimeline[i].Time > Clock.CurrentTime)
                    {
                        removeStartIndex = i;
                        break;
                    }
                }
                if (removeStartIndex > 0)
                    eventsTimeline.RemoveRange(removeStartIndex, eventsTimeline.Count - removeStartIndex);

                rewindFeverState();
            }
        }

        private void rewindFeverState()
        {
            var currentState = eventsTimeline.Last();
            ClearTransformsAfter(currentState.Time);

            // Find the last feverStart event to ensure that it is still active
            for (int i = 1; i <= eventsTimeline.Count; ++i)
            {
                if (eventsTimeline[^i].Type == FeverEvent.EventType.FeverStart)
                {
                    bool feverStillActive = eventsTimeline[^i].Time + fever_duration > Time.Current;
                    if (feverStillActive)
                    {
                        using (BeginAbsoluteSequence(eventsTimeline[^i].Time))
                            activateFever(byRewind: true);
                        FeverProgress.Value = (float)(1 - ((Time.Current - eventsTimeline[^i].Time) / fever_duration));
                        return;
                    }
                    else
                    {
                        FeverActivated.Value = false;
                    }
                    break;
                }
            }

            FeverProgress.Value = currentState.FeverProgress;
        }

        public void HandleResult(JudgementResult result)
        {
            if (FeverActivated.Value)
                return;

            FeverProgress.Value = Math.Min(FeverProgress.Value + feverIncreaseFor(result), 1);
            eventsTimeline.Add(new FeverEvent(result.TimeAbsolute, FeverProgress.Value, FeverEvent.EventType.Hit));
        }

        private float feverIncreaseFor(JudgementResult result)
        {
            if (result.Judgement is RushIgnoreJudgement) return 0;
            return (float)result.Judgement.NumericResultFor(result) / result.Judgement.MaxNumericResult / perfect_hits_to_fill;
        }

        private void activateFever(bool byRewind = false)
        {
            if (!byRewind)
                eventsTimeline.Add(new FeverEvent(Time.Current, 1, FeverEvent.EventType.FeverStart));
            FeverActivated.Value = true;
            this.TransformBindableTo(FeverProgress, 1).TransformBindableTo(FeverProgress, 0, fever_duration).Finally(_ => FeverActivated.Value = false);
        }

        public bool OnPressed(RushAction action)
        {
            if (action != RushAction.Fever)
                return false;

            if (FeverActivated.Value)
                return false;

            if (FeverProgress.Value < 1)
                return false;

            activateFever();
            return true;
        }

        public void OnReleased(RushAction action) { }

        private struct FeverEvent
        {
            public FeverEvent(double time, float feverProgress, EventType type)
            {
                Time = time;
                FeverProgress = feverProgress;
                Type = type;
            }

            public double Time;

            public float FeverProgress;

            public EventType Type;

            public enum EventType
            {
                SessionBegin,
                Hit,
                FeverStart,
            }
        }
    }
}
