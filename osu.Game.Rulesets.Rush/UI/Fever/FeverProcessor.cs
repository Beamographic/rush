// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Statistics;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Scoring;
using osu.Game.Utils;

namespace osu.Game.Rulesets.Rush.UI.Fever
{
    public class FeverProcessor : JudgementProcessor
    {
        private const float fever_duration = 5000;
        private const int perfect_hits_to_fill = 100;

        private static readonly GlobalStatistic<int> fever_periods_count = GlobalStatistics.Get<int>("rush!", "Fever Periods");

        private readonly List<Period> feverPeriods = new List<Period>();

        /// <summary>
        /// Whether the player is currently in fever mode.
        /// </summary>
        public Bindable<bool> InFeverMode = new Bindable<bool>();

        /// <summary>
        /// The amount of fever gathered by the player in percentage.
        /// </summary>
        public Bindable<float> FeverProgress = new BindableFloat
        {
            MinValue = 0.0f,
            MaxValue = 1.0f,
        };

        protected override void Update()
        {
            base.Update();

            // Reverse handling stuff:
            // This logic removes transforms of fever periods that are now in the future,
            // to be rewritten again by either the replay or the player themselves if that gets supported.
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

                if (removeStartIndex != -1)
                {
                    // This clears everything, and the current state is left as it is
                    ClearTransformsAfter(feverPeriods[removeStartIndex].Start);

                    // Reset to sane pre-fever state
                    InFeverMode.Value = false;
                    FeverProgress.Value = 1;

                    feverPeriods.RemoveRange(removeStartIndex, feverPeriods.Count - removeStartIndex);
                }

                // This only corrects the fever state if we are in one
                if (feverPeriods.Count > 0)
                {
                    var currentFeverPeriod = feverPeriods[^1];

                    if (Time.Current >= currentFeverPeriod.Start && Time.Current <= currentFeverPeriod.End)
                    {
                        ClearTransformsAfter(currentFeverPeriod.Start);
                        addFeverPeriodTransforms(currentFeverPeriod);
                    }
                }
            }

            fever_periods_count.Value = feverPeriods.Count;
        }

        protected override void ApplyResultInternal(JudgementResult result)
        {
            if (!(result is RushJudgementResult rushResult))
                throw new InvalidOperationException();

            rushResult.FeverProgressAtJudgement = FeverProgress.Value;

            if (!InFeverMode.Value)
                FeverProgress.Value += feverIncreaseFor(result);
        }

        protected override void RevertResultInternal(JudgementResult result)
        {
            if (!(result is RushJudgementResult rushResult))
                throw new InvalidOperationException();

            FeverProgress.Value = rushResult.FeverProgressAtJudgement;
        }

        /// <summary>
        /// Attempts to activate fever mode, and returns the attempt result.
        /// </summary>
        /// <returns>Whether fever has been activated or not.</returns>
        public bool TryActivateFever()
        {
            if (InFeverMode.Value)
                return false;

            if (FeverProgress.Value < 1)
                return false;

            activateFever();
            return true;
        }

        private void activateFever()
        {
            Debug.Assert(feverPeriods.Count == 0 || Time.Current > feverPeriods[^1].End);

            var feverPeriod = new Period(Time.Current, Time.Current + fever_duration);

            addFeverPeriodTransforms(feverPeriod);
            feverPeriods.Add(feverPeriod);
        }

        private void addFeverPeriodTransforms(Period period)
        {
            using (BeginAbsoluteSequence(period.Start, true))
            {
                this.TransformBindableTo(InFeverMode, true)
                    .TransformBindableTo(FeverProgress, 1)
                    .TransformBindableTo(FeverProgress, 0, period.End - period.Start).Then()
                    .TransformBindableTo(InFeverMode, false);
            }
        }

        private static float feverIncreaseFor(JudgementResult result)
        {
            if (result.Judgement is RushIgnoreJudgement || result.Judgement is RushFeverJudgement)
                return 0;

            return (float)result.Judgement.NumericResultFor(result) / result.Judgement.MaxNumericResult / perfect_hits_to_fill;
        }
    }
}
