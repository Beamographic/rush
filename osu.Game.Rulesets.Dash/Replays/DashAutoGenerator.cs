// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Game.Beatmaps;
using osu.Game.Replays;
using osu.Game.Rulesets.Dash.Objects;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Replays;

namespace osu.Game.Rulesets.Dash.Replays
{
    public class DashAutoGenerator : AutoGenerator
    {
        public const double RELEASE_DELAY = 20;
        private const double miniboss_punch_delay = 100;

        public new Beatmap<DashHitObject> Beatmap => (Beatmap<DashHitObject>)base.Beatmap;

        public DashAutoGenerator(IBeatmap beatmap)
            : base(beatmap)
        {
        }

        protected Replay Replay = new Replay();

        public override Replay Generate()
        {
            var pointGroups = generateActionPoints().GroupBy(a => a.Time).OrderBy(g => g.First().Time);

            var actions = new List<DashAction>();

            DashAction nextAir = DashAction.AirPrimary;
            DashAction nextGround = DashAction.GroundPrimary;

            foreach (var group in pointGroups)
            {
                foreach (var point in group)
                {
                    switch (point)
                    {
                        case HitPoint _:
                            actions.Add(point.Lane == LanedHitLane.Air ? nextAir : nextGround);
                            break;

                        case ReleasePoint _:
                            actions.Remove(point.Lane == LanedHitLane.Air ? nextAir : nextGround);
                            if (point.Lane == LanedHitLane.Air)
                                nextAir = nextAir == DashAction.AirPrimary ? DashAction.AirSecondary : DashAction.AirPrimary;
                            else
                                nextGround = nextGround == DashAction.GroundPrimary ? DashAction.GroundSecondary : DashAction.GroundPrimary;
                            break;
                    }
                }

                Replay.Frames.Add(new DashReplayFrame(group.First().Time, actions.ToArray()));
            }

            return Replay;
        }

        private IEnumerable<IActionPoint> generateActionPoints()
        {
            for (int i = 0; i < Beatmap.HitObjects.Count; i++)
            {
                if (!(Beatmap.HitObjects[i] is LanedHit lanedHit))
                    continue;

                LanedHit nextLanedHit = GetNextObject(i) as LanedHit;

                double endTime = lanedHit.GetEndTime();
                bool canDelayKeyUp = nextLanedHit == null ||
                                     nextLanedHit.StartTime > endTime + RELEASE_DELAY;

                double calculatedDelay = canDelayKeyUp ? RELEASE_DELAY : (nextLanedHit.StartTime - endTime) * 0.9;

                yield return new HitPoint { Time = lanedHit.StartTime, Lane = lanedHit.Lane };
                yield return new ReleasePoint { Time = endTime + calculatedDelay, Lane = lanedHit.Lane };
            }

            foreach (DashHitObject h in Beatmap.HitObjects)
            {
                switch (h)
                {
                    case LanedHit _:
                        break;

                    case MiniBoss miniBoss:
                        var currentTime = miniBoss.StartTime;
                        var endTime = miniBoss.GetEndTime();
                        var duration = endTime - currentTime;
                        var punchTime = Math.Min(duration / miniBoss.RequiredHits, miniboss_punch_delay + RELEASE_DELAY);

                        bool alternate = true;

                        while (currentTime < endTime)
                        {
                            yield return new HitPoint { Time = currentTime, Lane = alternate ? LanedHitLane.Ground : LanedHitLane.Air };

                            currentTime += punchTime;

                            yield return new ReleasePoint { Time = currentTime, Lane = alternate ? LanedHitLane.Ground : LanedHitLane.Air };

                            alternate = !alternate;
                        }

                        break;

                    default:
                        // TODO: other things
                        break;
                }
            }
        }

        protected override HitObject GetNextObject(int currentIndex)
        {
            if (!(Beatmap.HitObjects[currentIndex] is LanedHit lanedHit))
                return null;

            var desiredLane = lanedHit.Lane;

            for (int i = currentIndex + 1; i < Beatmap.HitObjects.Count; i++)
            {
                if (Beatmap.HitObjects[i] is LanedHit nextLanedHit && nextLanedHit.Lane == desiredLane)
                    return Beatmap.HitObjects[i];
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
