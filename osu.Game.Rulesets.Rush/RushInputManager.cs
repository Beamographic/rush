// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.ComponentModel;
using osu.Framework.Input.Bindings;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Rush
{
    public class RushInputManager : RulesetInputManager<RushAction>
    {
        /// <summary>
        /// Retrieves all actions in a currenty pressed states.
        /// </summary>
        public IEnumerable<RushAction> PressedActions => KeyBindingContainer.PressedActions;

        public RushInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }
    }

    public enum RushAction
    {
        [Description("Ground (Primary)")]
        GroundPrimary = 0,

        [Description("Ground (Secondary)")]
        GroundSecondary = 1,

        [Description("Ground (Tertiary)")]
        GroundTertiary = 2,

        [Description("Ground (Quaternary)")]
        GroundQuaternary = 3,

        [Description("Air (Primary)")]
        AirPrimary = 4,

        [Description("Air (Secondary)")]
        AirSecondary = 5,

        [Description("Air (Tertiary)")]
        AirTertiary = 6,

        [Description("Air (Quaternary)")]
        AirQuaternary = 7
    }

    public static class RushActionExtensions
    {
        public static LanedHitLane Lane(this RushAction action) => action switch
        {
            RushAction.GroundPrimary => LanedHitLane.Ground,
            RushAction.GroundSecondary => LanedHitLane.Ground,
            RushAction.GroundTertiary => LanedHitLane.Ground,
            RushAction.GroundQuaternary => LanedHitLane.Ground,
            RushAction.AirPrimary => LanedHitLane.Air,
            RushAction.AirSecondary => LanedHitLane.Air,
            RushAction.AirTertiary => LanedHitLane.Air,
            RushAction.AirQuaternary => LanedHitLane.Air,
            _ => LanedHitLane.Ground
        };
    }
}
