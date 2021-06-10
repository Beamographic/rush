// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Configuration;
using osu.Game.Rulesets.Configuration;

namespace osu.Game.Rulesets.Rush.Configuration
{
    public class RushRulesetConfigManager : RulesetConfigManager<RushRulsetSettings>
    {
        public RushRulesetConfigManager(SettingsStore settings, RulesetInfo ruleset, int? variant = null)
            : base(settings, ruleset, variant)
        {
        }

        protected override void InitialiseDefaults()
        {
            base.InitialiseDefaults();

            SetDefault(RushRulsetSettings.AutomaticFever, true);
        }
    }

    public enum RushRulsetSettings
    {
        AutomaticFever
    }
}
