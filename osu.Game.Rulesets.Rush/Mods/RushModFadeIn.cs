// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Rush.UI;

namespace osu.Game.Rulesets.Rush.Mods
{
    public class RushModFadeIn : RushModPlayfieldCover
    {
        public override string Name => "Fade In";
        public override string Acronym => "FI";

        public override string Description => @"Keys fade in before you hit them!";
        public override double ScoreMultiplier => 1;

        protected override CoverExpandDirection ExpandDirection => CoverExpandDirection.AlongScroll;
    }
}
