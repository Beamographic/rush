// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Rush.Beatmaps;

namespace osu.Game.Rulesets.Rush.Tests.Beatmaps
{
    [TestFixture]
    public class BeatmapConversionDecidingTest
    {
        [Test]
        public void TestCraftedTagCreatesCrafter()
            => Assert.IsTrue(converterForTag("crafted") is RushCraftedBeatmapConverter);

        [Test]
        public void TestWrongCasedCraftedTagCreatesCrafter()
            => Assert.IsTrue(converterForTag("cRafTeD") is RushCraftedBeatmapConverter);

        [Test]
        public void TestWordContainingCraftedTagCreatesGenerator()
            => Assert.IsTrue(converterForTag("handcraftedison") is RushGeneratedBeatmapConverter);

        [Test]
        public void TestRandomTagCreatesGenerator()
            => Assert.IsTrue(converterForTag("one two something") is RushGeneratedBeatmapConverter);

        [Test]
        public void TestNullTagCreatesGenerator()
            => Assert.IsTrue(converterForTag(null) is RushGeneratedBeatmapConverter);

        private IBeatmapConverter converterForTag(string tags)
        {
            var beatmap = new Beatmap { BeatmapInfo = { Metadata = { Tags = tags } } };
            var wrapper = new RushBeatmapConverter(beatmap, new RushRuleset());
            return wrapper.BackedConverter;
        }
    }
}
