// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
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

        private Drawable heldDrawable;

        protected override Drawable HandleButtonDown(InputState state, List<Drawable> targets)
        {
            heldDrawable = base.HandleButtonDown(state, targets);

            if (heldDrawable is IKeyBindingTouchHandler touchHandler)
                rushInputManager.TryPressTouchAction((TouchSource)Button, touchHandler.GetTargetActionFor(MouseDownPosition.Value));

            return heldDrawable;
        }

        protected override void HandleButtonUp(InputState state, List<Drawable> targets)
        {

            if (heldDrawable is IKeyBindingTouchHandler)
                rushInputManager.ReleaseTouchAction((TouchSource)Button);

            base.HandleButtonUp(state, targets);
        }
    }
}
