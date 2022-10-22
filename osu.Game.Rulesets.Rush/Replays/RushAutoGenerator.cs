// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Rush.Configuration;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Replays
{
    public class RushAutoGenerator : AutoGenerator
    {
        public const double RELEASE_DELAY = 20;

        public new Beatmap<RushHitObject> Beatmap => (Beatmap<RushHitObject>)base.Beatmap;

        public RushAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected Replay Replay = new Replay();

        public override Replay Generate()
        {
            var pointGroups = generateActionPoints().GroupBy(a => a.Time).OrderBy(g => g.First().Time);

            var actions = new List<RushAction>();

            RushAction nextAir = RushAction.AirPrimary;
            RushAction nextGround = RushAction.GroundPrimary;

            int airCount = 0, groundCount = 0;

            foreach (var group in pointGroups)
            {
                foreach (var point in group)
                {
                    switch (point)
                    {
                        case HitPoint _:
                            if (point.Lane == LanedHitLane.Air)
                            {
                                airCount++;
                                if (airCount == 1)
                                    actions.Add(nextAir);
                            }
                            else
                            {
                                groundCount++;
                                if (groundCount == 1)
                                    actions.Add(nextGround);
                            }

                            break;

                        case ReleasePoint _:
                            if (point.Lane == LanedHitLane.Air)
                            {
                                airCount--;

                                if (airCount == 0)
                                {
                                    actions.Remove(nextAir);
                                    nextAir = nextAir == RushAction.AirPrimary ? RushAction.AirSecondary : RushAction.AirPrimary;
                                }
                            }
                            else
                            {
                                groundCount--;

                                if (groundCount == 0)
                                {
                                    actions.Remove(nextGround);
                                    nextGround = nextGround == RushAction.GroundPrimary ? RushAction.GroundSecondary : RushAction.GroundPrimary;
                                }
                            }

                            break;
                    }
                }

                Replay.Frames.Add(new RushReplayFrame(group.First().Time, actions.ToArray())
                {
                    FeverActivationMode = FeverActivationMode.Automatic,
                });
            }

            return Replay;
        }

        private IEnumerable<IActionPoint> generateActionPoints()
        {
            const double sawblade_safety = 200;
            double lastAirHit = -1000, lastGroundHit = -1000;

            for (int i = 0; i < Beatmap.HitObjects.Count; i++)
            {
                var current = Beatmap.HitObjects[i];
                LanedHitLane? desiredLane;

                if (current is DualHit)
                    desiredLane = null;
                else if (current is Sawblade sawblade)
                {
                    desiredLane = sawblade.Lane.Opposite();
                    // don't bother if we're probably already in right lane
                    if (desiredLane == LanedHitLane.Air && current.StartTime < lastAirHit + sawblade_safety
                        || desiredLane == LanedHitLane.Ground && current.StartTime < lastGroundHit + sawblade_safety)
                        continue;
                }
                else if (current is LanedHit lanedHit)
                    desiredLane = lanedHit.Lane;
                else
                    continue;

                RushHitObject nextHit = GetNextObject(i) as RushHitObject;

                double endTime = current.GetEndTime();
                bool canDelayKeyUp = nextHit == null ||
                                     nextHit.StartTime > endTime + RELEASE_DELAY;

                double calculatedDelay = canDelayKeyUp ? RELEASE_DELAY : (nextHit.StartTime - endTime) * 0.9;

                if (desiredLane == null || desiredLane == LanedHitLane.Air)
                {
                    lastAirHit = endTime;
                    yield return new HitPoint { Time = current.StartTime, Lane = LanedHitLane.Air };
                    yield return new ReleasePoint { Time = endTime + calculatedDelay, Lane = LanedHitLane.Air };
                }

                if (desiredLane == null || desiredLane == LanedHitLane.Ground)
                {
                    lastGroundHit = endTime;
                    yield return new HitPoint { Time = current.StartTime, Lane = LanedHitLane.Ground };
                    yield return new ReleasePoint { Time = endTime + calculatedDelay, Lane = LanedHitLane.Ground };
                }
            }

            foreach (RushHitObject h in Beatmap.HitObjects)
            {
                switch (h)
                {
                    default:
                    case DualHit _:
                    case LanedHit _:
                        break;

                    case MiniBoss miniBoss:
                        var currentTime = miniBoss.StartTime;
                        var endTime = miniBoss.GetEndTime();
                        var duration = endTime - currentTime;
                        var punchTime = duration / (miniBoss.RequiredHits * 2f);

                        bool alternate = true;

                        while (currentTime < endTime)
                        {
                            yield return new HitPoint { Time = currentTime, Lane = alternate ? LanedHitLane.Ground : LanedHitLane.Air };

                            currentTime += punchTime / 2f;

                            yield return new ReleasePoint { Time = currentTime, Lane = alternate ? LanedHitLane.Ground : LanedHitLane.Air };

                            currentTime += punchTime / 2f;

                            alternate = !alternate;
                        }

                        break;
                }
            }
        }

        protected override HitObject GetNextObject(int currentIndex)
        {
            var current = Beatmap.HitObjects[currentIndex];
            LanedHitLane? desiredLane;

            if (current is DualHit)
                desiredLane = null;
            else if (Beatmap.HitObjects[currentIndex] is LanedHit lanedHit)
                desiredLane = lanedHit.Lane;
            else
                return null;

            for (int i = currentIndex + 1; i < Beatmap.HitObjects.Count; i++)
            {
                switch (Beatmap.HitObjects[i])
                {
                    case DualHit _:
                    case LanedHit nextLanedHit when desiredLane == null || nextLanedHit.Lane == desiredLane:
                        return Beatmap.HitObjects[i];
                }
            }

            return null;
        }

        private interface IActionPoint
        {
            double Time { get; set; }
            LanedHitLane Lane { get; set; }
        }

        private struct HitPoint : IActionPoint
        {
            public double Time { get; set; }
            public LanedHitLane Lane { get; set; }
        }

        private struct ReleasePoint : IActionPoint
        {
            public double Time { get; set; }
            public LanedHitLane Lane { get; set; }
        }
    }
}
