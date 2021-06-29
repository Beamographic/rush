// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.States;

namespace osu.Game.Rulesets.Rush.Input
{
    public class RushTouchEventManager : TouchEventManager
    {
        private readonly RushInputManager rushInputManager;
        public RushTouchEventManager(TouchSource source, RushInputManager inputManager)
            : base(source)
        {
            rushInputManager = inputManager;
        }

        protected override Drawable HandleButtonDown(InputState state, List<Drawable> targets)
        {
            base.HandleButtonDown(state, targets);

            if (HeldDrawable is IKeyBindingTouchHandler touchHandler)
                rushInputManager.TryPressTouchAction(Button, touchHandler.GetTargetActionFor(TouchDownPosition.Value));

            return HeldDrawable;
        }

        protected override void HandleButtonUp(InputState state, List<Drawable> targets)
        {

            if (HeldDrawable is IKeyBindingTouchHandler)
                rushInputManager.ReleaseTouchAction(Button);

            base.HandleButtonUp(state, targets);
        }
    }
}
