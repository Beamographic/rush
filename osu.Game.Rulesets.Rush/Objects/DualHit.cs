// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Threading;

namespace osu.Game.Rulesets.Rush.Objects
{
    public class DualHit : RushHitObject
    {
        public override double StartTime
        {
            get => base.StartTime;
            set
            {
                base.StartTime = value;
                Air.StartTime = Ground.StartTime = value;
            }
        }

        public readonly DualHitPart Air = new DualHitPart { Lane = LanedHitLane.Air };
        public readonly DualHitPart Ground = new DualHitPart { Lane = LanedHitLane.Ground };

        protected override void CreateNestedHitObjects(CancellationToken cancellationToken)
        {
            base.CreateNestedHitObjects(cancellationToken);

            AddNested(Air);
            AddNested(Ground);
        }
    }
}
