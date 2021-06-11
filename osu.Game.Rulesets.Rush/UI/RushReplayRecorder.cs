// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Game.Rulesets.Replays;
using osu.Game.Rulesets.Rush.Configuration;
using osu.Game.Rulesets.Rush.Replays;
using osu.Game.Rulesets.UI;
using osu.Game.Scoring;
using osuTK;

namespace osu.Game.Rulesets.Rush.UI
{
    public class RushReplayRecorder : ReplayRecorder<RushAction>
    {
        private readonly Bindable<bool> automaticFever = new Bindable<bool>(true);

        public RushReplayRecorder(Score target)
            : base(target)
        {
        }

        [BackgroundDependencyLoader(true)]
        private void load(RushRulesetConfigManager rushConfigs)
        {
            rushConfigs?.BindWith(RushRulesetSettings.AutomaticFever, automaticFever);
        }

        protected override ReplayFrame HandleFrame(Vector2 mousePosition, List<RushAction> actions, ReplayFrame previousFrame)
            => new RushReplayFrame(Time.Current, actions, automaticFever.Value);
    }
}
