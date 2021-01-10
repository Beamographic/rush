// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using System.Threading;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;
using osu.Game.Rulesets.Rush.Judgements;
using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class NoteSheet : LanedHit, IHasDuration
    {
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

        public readonly NoteSheetHead Head = new NoteSheetHead();

        public readonly NoteSheetTail Tail = new NoteSheetTail();

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            AddNested(Head);
            AddNested(Tail);

            updateNestedSamples();
        }

        private void updateNestedSamples()
        {
            if (Samples.Count == 0)
                return;

            Head.Samples = Samples.Take(1).ToList();
            Tail.Samples = Samples.TakeLast(1).ToList();
        }

        public override Judgement CreateJudgement() => new RushJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
