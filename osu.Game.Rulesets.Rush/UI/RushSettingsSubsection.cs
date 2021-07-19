// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Localisation;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Rush.Configuration;

namespace osu.Game.Rulesets.Rush.UI
{
    public class RushSettingsSubsection : RulesetSettingsSubsection
    {
        private readonly Ruleset ruleset;

        protected new RushRulesetConfigManager Config => (RushRulesetConfigManager)base.Config;

        protected override LocalisableString Header => ruleset.Description;

        public RushSettingsSubsection(Ruleset ruleset)
            : base(ruleset)
        {
            this.ruleset = ruleset;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                new SettingsEnumDropdown<FeverActivationMode>
                {
                    LabelText = "Fever activation mode",
                    Current = Config.GetBindable<FeverActivationMode>(RushRulesetSettings.FeverActivationMode)
                },
            };
        }
    }
}
