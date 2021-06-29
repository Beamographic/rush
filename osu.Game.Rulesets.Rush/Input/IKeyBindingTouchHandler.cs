// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK;

namespace osu.Game.Rulesets.Rush.Input
{
    public interface IKeyBindingTouchHandler
    {
        TargetAction GetTargetActionFor(Vector2 screenSpaceTouchPosition) => TargetAction.None;
    }

    public enum TargetAction
    {
        None,
        Ground,
        Air,
        Fever,
    }
}
