// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Rush.Beatmaps;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.UI;
using osuTK.Input;

namespace osu.Game.Rulesets.Rush.Tests.Visual
{
    public partial class TestSceneMiniBoss : TestSceneRushPlayer
    {
        private const float mini_boss_time = 600f;

        protected override IBeatmap CreateBeatmap(RulesetInfo ruleset)
        {
            var beatmap = new RushBeatmap();

            for (int i = 0; i < mini_boss_time / 200f; i++)
            {
                beatmap.HitObjects.Add(new Minion
                {
                    StartTime = i * 200,
                });
            }

            beatmap.HitObjects.Add(new MiniBoss
            {
                StartTime = mini_boss_time,
                Duration = 1000f,
            });

            return beatmap;
        }

        private PlayerTargetLane targetLane => ((DrawableRushRuleset)Player.DrawableRuleset).Playfield.PlayerSprite.Target;

        [Test]
        public void TestPlayerStateTransitionsAtCorrectTime()
        {
            AddStep("mash air actions", () => Scheduler.AddDelayed(() =>
            {
                InputManager.Click(MouseButton.Left);
            }, 50f, true));

            AddAssert("not in fight state", () => targetLane != PlayerTargetLane.MiniBoss);
            AddUntilStep("wait for fight state", () => targetLane == PlayerTargetLane.MiniBoss);
            AddAssert("time has reached mini-boss time", () => Player.GameplayClockContainer.CurrentTime >= mini_boss_time);

            AddStep("stop mashing", () => Scheduler.CancelDelayedTasks());
        }
    }
}
