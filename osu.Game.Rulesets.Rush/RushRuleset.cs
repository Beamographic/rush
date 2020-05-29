// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Rush.Beatmaps;
using osu.Game.Rulesets.Rush.Mods;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Rush
{
    public class RushRuleset : Ruleset
    {
        public override string Description => "rush";

        public override string PlayingVerb => "Punching doods";

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawableRushRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new RushBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) => new RushDifficultyCalculator(this, beatmap);

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new[] { new RushModNoFail() };

                case ModType.Automation:
                    return new[] { new RushModAutoplay() };

                default:
                    return new Mod[] { null };
            }
        }

        public override string ShortName => "rush";

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.J, RushAction.GroundPrimary),
            new KeyBinding(InputKey.K, RushAction.GroundSecondary),
            new KeyBinding(InputKey.F, RushAction.AirPrimary),
            new KeyBinding(InputKey.D, RushAction.AirSecondary),
            new KeyBinding(InputKey.MouseRight, RushAction.GroundPrimary),
            new KeyBinding(InputKey.MouseLeft, RushAction.AirPrimary),
        };

        public override Drawable CreateIcon() => new RushIcon();

        public class RushIcon : CompositeDrawable
        {
            public RushIcon()
            {
                AutoSizeAxes = Axes.Both;

                InternalChildren = new Drawable[]
                {
                    new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Icon = FontAwesome.Regular.Circle,
                    },
                    new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Scale = new Vector2(0.6f),
                        Icon = FontAwesome.Solid.Running,
                    }
                };
            }
        }
    }
}
