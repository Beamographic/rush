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
            => Assert.That(converterForTag("crafted") is RushCraftedBeatmapConverter, Is.True);

        [Test]
        public void TestWrongCasedCraftedTagCreatesCrafter()
            => Assert.That(converterForTag("cRafTeD") is RushCraftedBeatmapConverter, Is.True);

        [Test]
        public void TestWordContainingCraftedTagCreatesGenerator()
            => Assert.That(converterForTag("handcraftedison") is RushGeneratedBeatmapConverter, Is.True);

        [Test]
        public void TestRandomTagCreatesGenerator()
            => Assert.That(converterForTag("one two something") is RushGeneratedBeatmapConverter, Is.True);

        [Test]
        public void TestNullTagCreatesGenerator()
            => Assert.That(converterForTag(null) is RushGeneratedBeatmapConverter, Is.True);

        private IBeatmapConverter converterForTag(string tags)
        {
            var beatmap = new Beatmap { BeatmapInfo = { Metadata = { Tags = tags } } };
            var wrapper = new RushBeatmapConverter(beatmap, new RushRuleset());
            return wrapper.BackedConverter;
        }
    }
}
