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
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.UI.Fever
{
    public class FeverTracker2 : JudgementProcessor, IKeyBindingHandler<RushAction>
    {
        private const float fever_duration = 10000;
        public Bindable<bool> FeverActivated = new Bindable<bool>();
        public Bindable<float> FeverProgress = new Bindable<float>();

        private List<double> feverStartTimes = new List<double>();
        private List<float> progressAtFever = new List<float>();

        private const int perfect_hits_to_fill = 50;

        protected override void Update()
        {
            base.Update();

            // Reverse handling stuff
            if (Clock.Rate < 0)
            {
                int removeStartIndex = -1;
                for (int i = 0; i < feverStartTimes.Count; ++i)
                {
                    if (feverStartTimes[i] > Clock.CurrentTime)
                    {
                        removeStartIndex = i;
                        break;
                    }
                }
                // Our time is now before a fever, ensure a sensible progress value is in place
                if (removeStartIndex != -1)
                {
                    FinishTransforms(); // End the current fever if there's any

                    // We must reset to the exact progress value used at the time, else the DHO reverts will desync the fever state
                    FeverProgress.Value = progressAtFever[removeStartIndex];

                    feverStartTimes.RemoveRange(removeStartIndex, feverStartTimes.Count - removeStartIndex);
                    progressAtFever.RemoveRange(removeStartIndex, progressAtFever.Count - removeStartIndex);
                }

                // Correct current fever state if applicable
                if (!feverStartTimes.Any())
                    return;

                var currentFeverStartTime = feverStartTimes.Last();

                if (Time.Current < currentFeverStartTime + fever_duration) // We are within a fever period
                    activateFeverAtPeriod(currentFeverStartTime, currentFeverStartTime + fever_duration);
            }
        }

        protected override void ApplyResultInternal(JudgementResult result)
        {
            if (FeverActivated.Value)
                return;

            FeverProgress.Value += feverIncreaseFor(result);
        }

        protected override void RevertResultInternal(JudgementResult result)
        {
            if (FeverActivated.Value)
                return;

            FeverProgress.Value = Math.Max(FeverProgress.Value - feverIncreaseFor(result), 0);
        }

        private void activateFeverAtPeriod(double startTime, double endTime)
        {
            // We ensure no fever state is running
            FinishTransforms(true);

            using (BeginAbsoluteSequence(startTime, true))
                this.TransformBindableTo(FeverActivated, true).TransformBindableTo(FeverProgress, 1).TransformBindableTo(FeverProgress, 0, endTime - startTime).Then().TransformBindableTo(FeverActivated, false);
        }

        private void activateNewFeverAt(double startTime)
        {
            feverStartTimes.Add(Time.Current);
            progressAtFever.Add(FeverProgress.Value);

            activateFeverAtPeriod(startTime, startTime + fever_duration);
        }

        private float feverIncreaseFor(JudgementResult result)
        {
            if (result.Judgement is RushIgnoreJudgement || result.Judgement is RushFeverJudgement) return 0;
            return (float)result.Judgement.NumericResultFor(result) / result.Judgement.MaxNumericResult / perfect_hits_to_fill;
        }

        public bool OnPressed(RushAction action)
        {
            if (action != RushAction.Fever)
                return false;

            if (FeverActivated.Value)
                return false;

            if (FeverProgress.Value < 1)
                return false;

            activateNewFeverAt(Time.Current);
            return true;
        }

        public void OnReleased(RushAction action) { }

    }
}
