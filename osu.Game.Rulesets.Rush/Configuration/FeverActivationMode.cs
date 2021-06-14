// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace osu.Game.Rulesets.Rush.Configuration
{
    /// <summary>
    /// The activation mode of fever for gaining bonus score on hits.
    /// </summary>
    public enum FeverActivationMode
    {
        /// <summary>
        /// Automatically once the fever progress fills up.
        /// </summary>
        Automatic,

        /// <summary>
        /// Manually using the <see cref="RushAction.Fever"/> action.
        /// </summary>
        Manual,
    }
}
