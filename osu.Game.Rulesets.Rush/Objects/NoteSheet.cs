// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Objects.Types;
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
                Body.Duration = value;
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
                Body.StartTime = value;
                Body.EndTime = EndTime;
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
                Body.Lane = value;
            }
        }

        public readonly NoteSheetHead Head = new NoteSheetHead();

        public readonly NoteSheetTail Tail = new NoteSheetTail();

        public readonly NoteSheetBody Body = new NoteSheetBody();

        protected override void CreateNestedHitObjects()
        {
            base.CreateNestedHitObjects();

            AddNested(Body);
            AddNested(Head);
            AddNested(Tail);
        }

        public override Judgement CreateJudgement() => new IgnoreJudgement();

        protected override HitWindows CreateHitWindows() => HitWindows.Empty;
    }
}
