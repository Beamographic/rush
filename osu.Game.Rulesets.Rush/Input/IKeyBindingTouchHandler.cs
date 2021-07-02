// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osuTK;

namespace osu.Game.Rulesets.Rush.Input
{
    public interface IKeyBindingTouchHandler
    {
        RushActionTarget ActionTargetForTouchPosition(Vector2 screenSpaceTouchPos) => RushActionTarget.None;
    }

    public enum RushActionTarget
    {
        None,
        Ground,
        Air,
        Fever,
    }
}
