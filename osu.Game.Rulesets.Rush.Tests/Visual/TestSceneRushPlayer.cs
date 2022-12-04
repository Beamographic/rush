// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Tests.Visual;

namespace osu.Game.Rulesets.Rush.Tests.Visual
{
    [TestFixture]
    public partial class TestSceneRushPlayer : PlayerTestScene
    {
        private readonly BindableBool pausedBindable = new BindableBool();

        protected new RushPlayer Player => (RushPlayer)base.Player;

        protected override TestPlayer CreatePlayer(Ruleset ruleset) => new RushPlayer();

        protected override Ruleset CreatePlayerRuleset() => new RushRuleset();

        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new OsuCheckbox
            {
                LabelText = "Pause",
                RelativeSizeAxes = Axes.None,
                AutoSizeAxes = Axes.Y,
                Width = 100,
                Origin = Anchor.TopLeft,
                Anchor = Anchor.TopLeft,
                Margin = new MarginPadding { Top = 40f, Left = 10f },
                Depth = float.NegativeInfinity,
                Current = { BindTarget = pausedBindable }
            });

            pausedBindable.ValueChanged += e => Player?.SetGameplayClockPaused(e.NewValue);
        }

        protected partial class RushPlayer : TestPlayer
        {
            public void SetGameplayClockPaused(bool value)
            {
                if (GameplayClockContainer.IsPaused.Value && !value)
                    GameplayClockContainer.Start();
                else if (!GameplayClockContainer.IsPaused.Value && value)
                    GameplayClockContainer.Stop();
            }
        }
    }
}
