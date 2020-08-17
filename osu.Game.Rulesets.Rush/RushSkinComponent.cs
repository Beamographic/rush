// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Rush
{
    public class RushSkinComponent : GameplaySkinComponent<RushSkinComponents>
    {
        public readonly LanedHitLane? Lane;

        public RushSkinComponent(RushSkinComponents component, LanedHitLane? lane = null)
            : base(component)
        {
            Lane = lane;
        }

        protected override string RulesetPrefix => RushRuleset.SHORT_NAME;
        protected override string ComponentName => Component.ToString().ToLower();
    }
}
