// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.States;
using osuTK.Input;

namespace osu.Game.Rulesets.Rush.Input
{
    public class RushMouseEventManager : MouseButtonEventManager
    {
        public override bool EnableClick => true;
        public override bool EnableDrag => false;
        public override bool ChangeFocusOnClick => false;

        private readonly RushInputManager rushInputManager;
        public RushMouseEventManager(MouseButton source, RushInputManager inputManager)
            : base(source)
        {
            rushInputManager = inputManager;
        }

        private IKeyBindingTouchHandler touchHandler;

        protected override Drawable HandleButtonDown(InputState state, List<Drawable> targets)
        {
            touchHandler = targets.FirstOrDefault(d => d is IKeyBindingTouchHandler) as IKeyBindingTouchHandler;

            if (touchHandler != null)
                rushInputManager.TryPressTouchAction((TouchSource)Button, touchHandler.GetTargetActionFor(MouseDownPosition.Value));

            return base.HandleButtonDown(state, targets);
        }

        protected override void HandleButtonUp(InputState state, List<Drawable> targets)
        {
            if (touchHandler != null)
                rushInputManager.ReleaseTouchAction((TouchSource)Button);

            touchHandler = null;

            base.HandleButtonUp(state, targets);
        }
    }
}
