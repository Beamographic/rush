// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using osu.Game.Audio;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class StarSheet : LanedHit, IHasDuration
    {
        public List<IList<HitSampleInfo>> NodeSamples;

        public double EndTime
        {
            get => StartTime + Duration;
            set => Duration = value - StartTime;
        }

        private double duration;

        public double Duration
        {
            get => duration;
            set
            {
                duration = value;
                Tail.StartTime = EndTime;
            }
        }

        public override double StartTime
        {
            get => base.StartTime;
            set
            {
                base.StartTime = value;
                Head.StartTime = value;
                Tail.StartTime = EndTime;
            }
        }

        public override LanedHitLane Lane
        {
            get => base.Lane;
            set
            {
                base.Lane = value;
                Head.Lane = value;
                Tail.Lane = value;
            }
        }

        public readonly StarSheetHead Head = new StarSheetHead();

        public readonly StarSheetTail Tail = new StarSheetTail();

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            AddNested(Head);
            AddNested(Tail);

            updateNestedSamples();
        }

        private void updateNestedSamples()
        {
            if (NodeSamples.Count == 0)
                return;

            Head.Samples = NodeSamples.First();
            Tail.Samples = NodeSamples.Last();
        }

        public override Judgement CreateJudgement() => new RushIgnoreJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;

        protected override bool HasFeverBonus => false;
    }
}
