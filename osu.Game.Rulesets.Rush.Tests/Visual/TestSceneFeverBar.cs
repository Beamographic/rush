// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Rush.UI.Fever;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Rush.Tests.Visual
{
    public class TestSceneFeverBar : OsuTestScene
    {
        [Cached]
        private readonly FeverProcessor feverProcessor = new FeverProcessor();

        public TestSceneFeverBar()
        {
            Children = new Drawable[]
            {
                feverProcessor,
                new FeverBar()
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Y = 0,
                }
            };

            AddSliderStep<float>("Progress", 0, 1, 0, v => feverProcessor.FeverProgress.Value = v);
            AddToggleStep("Activated", v => feverProcessor.InFeverMode.Value = v);
        }
    }
}
