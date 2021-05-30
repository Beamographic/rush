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
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.UI.Fever
{
    public class FeverTracker2 : JudgementProcessor, IKeyBindingHandler<RushAction>
    {
        public override bool RemoveCompletedTransforms => false;

        private const float fever_duration = 10000;
        public Bindable<bool> FeverActivated = new Bindable<bool>();
        public Bindable<float> FeverProgress = new Bindable<float>();

        private List<double> feverStartTimes = new List<double>();

        private const int perfect_hits_to_fill = 50;

        protected override void Update()
        {
            base.Update();
            if (Clock.Rate < 0)
            {
                ApplyTransformsAt(Time.Current, true);
                ClearTransformsAfter(Time.Current, true);
                // Reverse handling stuff
                int removeStartIndex = -1;
                for (int i = 0; i < feverStartTimes.Count; ++i)
                {
                    if (feverStartTimes[i] > Clock.CurrentTime)
                    {
                        removeStartIndex = i;
                        break;
                    }
                }
                if (removeStartIndex != -1)
                {
                    // Revert fever transforms, and potentially end the current active fever
                    FeverActivated.Value = false;
                    feverStartTimes.RemoveRange(removeStartIndex, feverStartTimes.Count - removeStartIndex);
                }
                // Correct current fever state if applicable
                rewindFeverState();
            }
        }

        private void rewindFeverState()
        {
            if (!feverStartTimes.Any())
                return;
            var currentFeverStartTime = feverStartTimes.Last();
            if (Time.Current < currentFeverStartTime + fever_duration)
            {
                // Revert fever transforms, and potentially end the current active fever
                ApplyTransformsAt(currentFeverStartTime, true);
                ClearTransformsAfter(currentFeverStartTime);
                // We are in fever mode
                using (BeginAbsoluteSequence(currentFeverStartTime, true))
                    activateFever(true);
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

        private void activateFever(bool byRewind = false)
        {
            if (!byRewind)
                feverStartTimes.Add(Time.Current);
            FeverActivated.Value = true;
            this.TransformBindableTo(FeverProgress, 1).TransformBindableTo(FeverProgress, 0, fever_duration).Finally(_ => FeverActivated.Value = false);
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

            activateFever();
            return true;
        }

        public void OnReleased(RushAction action) { }

    }
}
