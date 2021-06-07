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
using osu.Game.Utils;

namespace osu.Game.Rulesets.Rush.UI.Fever
{
    public class FeverProcessor : JudgementProcessor, IKeyBindingHandler<RushAction>
    {
        private const float fever_duration = 5000;
        private const int perfect_hits_to_fill = 100;

        public Bindable<bool> InFeverMode = new Bindable<bool>();

        public Bindable<float> FeverProgress = new BindableFloat
        {
            MinValue = 0.0f,
            MaxValue = 1.0f,
        };

        private readonly List<Period> feverPeriods = new List<Period>();

        protected override void Update()
        {
            base.Update();

            // Reverse handling stuff
            if (Clock.Rate < 0)
            {
                int removeStartIndex = -1;

                for (int i = 0; i < feverPeriods.Count; ++i)
                {
                    if (Time.Current < feverPeriods[i].Start)
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
                    FeverProgress.Value = feverStack.Peek();

                    feverPeriods.RemoveRange(removeStartIndex, feverPeriods.Count - removeStartIndex);
                }

                if (!feverPeriods.Any())
                    return;

                var currentFeverPeriod = feverPeriods.Last();
                if (Time.Current < currentFeverPeriod.End) // We are within a fever period
                    activateFeverAtPeriod(currentFeverPeriod);
            }
        }

        protected override void ApplyResultInternal(JudgementResult result)
        {
            if (!(result is RushJudgementResult rushResult))
                throw new InvalidOperationException();

            if (InFeverMode.Value)
                return;

            rushResult.FeverProgressAtJudgement = FeverProgress.Value;
            FeverProgress.Value += feverIncreaseFor(result);
        }

        protected override void RevertResultInternal(JudgementResult result)
        {
            if (!(result is RushJudgementResult rushResult))
                throw new InvalidOperationException();

            if (InFeverMode.Value)
                return;

            FeverProgress.Value = rushResult.FeverProgressAtJudgement;
        }

        private void activateFeverAtPeriod(Period period)
        {
            // We ensure no fever state is running
            ClearTransforms(true);

            using (BeginAbsoluteSequence(period.Start, true))
                this.TransformBindableTo(InFeverMode, true)
                    .TransformBindableTo(FeverProgress, 1)
                    .TransformBindableTo(FeverProgress, 0, period.End - period.Start).Then()
                    .TransformBindableTo(InFeverMode, false);
        }

        private void activateNewFever()
        {
            var feverPeriod = new Period(Time.Current, Time.Current + fever_duration);

            feverPeriods.Add(feverPeriod);
            activateFeverAtPeriod(feverPeriod);
        }

        private static float feverIncreaseFor(JudgementResult result)
        {
            if (result.Judgement is RushIgnoreJudgement || result.Judgement is RushFeverJudgement)
                return 0;

            return (float)result.Judgement.NumericResultFor(result) / result.Judgement.MaxNumericResult / perfect_hits_to_fill;
        }

        public bool OnPressed(RushAction action)
        {
            if (action != RushAction.Fever)
                return false;

            if (InFeverMode.Value)
                return false;

            if (FeverProgress.Value < 1)
                return false;

            activateNewFever();
            return true;
        }

        public void OnReleased(RushAction action)
        {
        }
    }
}
