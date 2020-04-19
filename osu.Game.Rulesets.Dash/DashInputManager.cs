// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Dash
{
    public class DashInputManager : RulesetInputManager<DashAction>
    {
        public DashInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum DashAction
    {
        [Description("Ground (Primary)")]
        GroundPrimary,

        [Description("Ground (Secondary)")]
        GroundSecondary,

        [Description("Air (Primary)")]
        AirPrimary,

        [Description("Air (Secondary)")]
        AirSecondary
    }
}
