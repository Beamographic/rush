// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.Rush.Objects
{
    public class DualOrb : RushHitObject
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

        public readonly Orb Air = new Orb { Lane = LanedHitLane.Air };
        public readonly Orb Ground = new Orb { Lane = LanedHitLane.Ground };

        protected override void CreateNestedHitObjects()
        {
            base.CreateNestedHitObjects();

            AddNested(Air);
            AddNested(Ground);
        }
    }
}
