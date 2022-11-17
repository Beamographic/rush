// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Skinning;

namespace osu.Game.Rulesets.Rush
{
    public class RushSkinComponent : GameplaySkinComponentLookup<RushSkinComponents>
    {
        public RushSkinComponent(RushSkinComponents component)
            : base(component)
        {
        }

        protected override string RulesetPrefix => RushRuleset.SHORT_NAME;
        protected override string ComponentName => Component.ToString().ToLower();
    }
}
