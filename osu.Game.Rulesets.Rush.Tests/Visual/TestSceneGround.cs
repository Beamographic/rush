// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Graphics;
using osu.Game.Rulesets.Rush.UI.Ground;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Rush.Tests.Visual
{
    public partial class TestSceneGround : OsuTestScene
    {
        public TestSceneGround()
        {
            Children = new Drawable[]
            {
                new GroundDisplay
                {
                    RelativePositionAxes = Axes.Y,
                    Y = 0.7f,
                },
            };
        }
    }
}
