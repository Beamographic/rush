// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Rush.Replays;

namespace osu.Game.Rulesets.Rush.Tests.Replay
{
    [TestFixture]
    public class RushReplayFrameTest
    {
        [TestCase]
        [TestCase(RushAction.AirPrimary, RushAction.GroundPrimary)]
        [TestCase(RushAction.AirPrimary, RushAction.AirSecondary, RushAction.GroundQuaternary)]
        [TestCase(RushAction.GroundPrimary, RushAction.GroundSecondary)]
        [TestCase(RushAction.AirPrimary, RushAction.AirSecondary, RushAction.AirTertiary, RushAction.AirQuaternary, RushAction.GroundPrimary, RushAction.GroundSecondary, RushAction.GroundTertiary, RushAction.GroundTertiary)]
        public void TestParity(params RushAction[] actions)
        {
            var originalFrame = (RushReplayFrame)new RushRuleset().CreateConvertibleReplayFrame();
            var resultFrame = (RushReplayFrame)new RushRuleset().CreateConvertibleReplayFrame();

            resultFrame.FromLegacy(originalFrame.ToLegacy(new Beatmap()), new Beatmap());

            Assert.That(resultFrame.Actions, Is.EquivalentTo(originalFrame.Actions));
        }
    }
}
