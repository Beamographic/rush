// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Dash.Beatmaps;
using osu.Game.Rulesets.Dash.Mods;
using osu.Game.Rulesets.Dash.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Dash
{
    public class DashRuleset : Ruleset
    {
        public override string Description => "osu!dash";

        public override string PlayingVerb => "Punching doods";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawableDashRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new DashBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) => new DashDifficultyCalculator(this, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new[] { new DashModNoFail() };

                case ModType.Automation:
                    return new[] { new DashModAutoplay() };

                default:
                    return new Mod[] { null };
            }
        }

        public override string ShortName => "dash";

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.J, DashAction.GroundPrimary),
            new KeyBinding(InputKey.K, DashAction.GroundSecondary),
            new KeyBinding(InputKey.F, DashAction.AirPrimary),
            new KeyBinding(InputKey.D, DashAction.AirSecondary),
            new KeyBinding(InputKey.MouseRight, DashAction.GroundPrimary),
            new KeyBinding(InputKey.MouseLeft, DashAction.AirPrimary),
        };

        public override Drawable CreateIcon() => new SpriteText
        {
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Text = ShortName[0].ToString(),
            Font = OsuFont.Default.With(size: 18),
        };
    }
}
