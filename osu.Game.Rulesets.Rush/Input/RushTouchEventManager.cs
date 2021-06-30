// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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

        private IKeyBindingTouchHandler touchHandler;

        protected override Drawable HandleButtonDown(InputState state, List<Drawable> targets)
        {
            touchHandler = targets.FirstOrDefault(d => d is IKeyBindingTouchHandler) as IKeyBindingTouchHandler;

            if (touchHandler != null)
                rushInputManager.TryPressTouchAction(Button, touchHandler.GetTargetActionFor(TouchDownPosition.Value));

            return base.HandleButtonDown(state, targets);
        }

        protected override void HandleButtonUp(InputState state, List<Drawable> targets)
        {
            if (touchHandler != null)
                rushInputManager.ReleaseTouchAction(Button);

            touchHandler = null;
            base.HandleButtonUp(state, targets);
        }
    }
}
