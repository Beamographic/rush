// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.ComponentModel;
using osu.Framework.Extensions.ListExtensions;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Framework.Lists;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Replays;
using osu.Game.Rulesets.UI;
using osuTK.Input;

namespace osu.Game.Rulesets.Rush.Input
{
    public class RushInputManager : RulesetInputManager<RushAction>
    {
        protected override bool MapMouseToLatestTouch => false;
        public new RushFramedReplayInputHandler ReplayInputHandler => (RushFramedReplayInputHandler)base.ReplayInputHandler;

        /// <summary>
        /// Retrieves all actions in a currenty pressed states.
        /// </summary>
        public SlimReadOnlyListWrapper<RushAction> PressedActions => ((List<RushAction>)KeyBindingContainer.PressedActions).AsSlimReadOnly();

        public RushInputManager(RulesetInfo ruleset)
            : base(ruleset, 0, SimultaneousBindingMode.Unique)
        {
        }

        protected override MouseButtonEventManager CreateButtonEventManagerFor(MouseButton button)
            => new RushMouseEventManager(button, this);

        protected override TouchEventManager CreateButtonEventManagerFor(TouchSource source)
            => new RushTouchEventManager(source, this);


        private readonly Dictionary<TouchSource, RushAction> touchActionMap = new Dictionary<TouchSource, RushAction>();

        private readonly Dictionary<RushAction, bool> actionsInUse = new Dictionary<RushAction, bool>();

        private void updateActionsCache()
        {
            for (RushAction action = RushAction.GroundPrimary; action <= RushAction.Fever; ++action)
                actionsInUse[action] = false;

            foreach (var pressedAction in PressedActions)
                actionsInUse[pressedAction] = true;
        }

        private RushAction? tryGetGroundAction() => tryGetActionInRange(RushAction.GroundPrimary, RushAction.AirPrimary);
        private RushAction? tryGetAirAction() => tryGetActionInRange(RushAction.AirPrimary, RushAction.Fever);
        private RushAction? tryGetFeverAction()
        {
            actionsInUse.TryGetValue(RushAction.Fever, out var inUse);

            if (!inUse) return RushAction.Fever;

            return null;
        }
        private RushAction? tryGetActionInRange(RushAction lowerBound, RushAction upperBound)
        {
            for (RushAction a = lowerBound; a < upperBound; ++a)
            {
                actionsInUse.TryGetValue(a, out var inUse);

                if (!inUse) return a;
            }

            return null;
        }

        public bool TryPressTouchAction(TouchSource source, RushActionTarget action)
        {
            updateActionsCache();
            RushAction? convertedAction = action switch
            {
                RushActionTarget.Ground => tryGetGroundAction(),
                RushActionTarget.Air => tryGetAirAction(),
                RushActionTarget.Fever => tryGetFeverAction(),
                _ => null
            };

            if (convertedAction is null) return false;

            touchActionMap[source] = convertedAction.Value;
            KeyBindingContainer.TriggerPressed(convertedAction.Value);

            return true;
        }

        public void ReleaseTouchAction(TouchSource source)
        {
            if (!touchActionMap.TryGetValue(source, out var action)) return;
            // The action has already been released, maybe due to the same keyboard key being released before the touch releases.
            if (!PressedActions.Contains(action)) return;

            KeyBindingContainer.TriggerReleased(action);
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
        AirQuaternary = 7,

        [Description("Activate fever")]
        Fever = 8,
    }

    public static class RushActionExtensions
    {
        public static bool IsLaneAction(this RushAction action) => action < RushAction.Fever;

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
