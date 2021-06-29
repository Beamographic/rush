// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Bindings;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Overlays.Settings;
using osu.Game.Rulesets.Configuration;
using osu.Game.Rulesets.Difficulty;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Replays.Types;
using osu.Game.Rulesets.Rush.Beatmaps;
using osu.Game.Rulesets.Rush.Configuration;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Rush.Mods;
using osu.Game.Rulesets.Rush.Replays;
using osu.Game.Rulesets.Rush.Scoring;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Rush
{
    public class RushRuleset : Ruleset
    {
        public const string SHORT_NAME = "rush";

        private static readonly Lazy<bool> is_development_build = new Lazy<bool>(() => typeof(RushRuleset).Assembly.GetName().Name.EndsWith("-dev"));

        public static bool IsDevelopmentBuild => is_development_build.Value;

        public override string ShortName => !IsDevelopmentBuild ? SHORT_NAME : $"{SHORT_NAME}-dev";

        public override string Description => !IsDevelopmentBuild ? "Rush!" : "Rush! (dev build)";

        public override string PlayingVerb => "Punching doods";

        public override RulesetSettingsSubsection CreateSettings() => new RushSettingsSubsection(this);

        public override IRulesetConfigManager CreateConfig(SettingsStore settings) => new RushRulesetConfigManager(settings, RulesetInfo);

        public override DrawableRuleset CreateDrawableRulesetWith(IBeatmap beatmap, IReadOnlyList<Mod> mods = null) => new DrawableRushRuleset(this, beatmap, mods);

        public override IBeatmapConverter CreateBeatmapConverter(IBeatmap beatmap) => new RushBeatmapConverter(beatmap, this);

        public override DifficultyCalculator CreateDifficultyCalculator(WorkingBeatmap beatmap) => new RushDifficultyCalculator(this, beatmap);

        public override HealthProcessor CreateHealthProcessor(double drainStartTime) => new RushHealthProcessor();

        public override ScoreProcessor CreateScoreProcessor() => new RushScoreProcessor();

        public override IEnumerable<Mod> GetModsFor(ModType type)
        {
            switch (type)
            {
                case ModType.DifficultyReduction:
                    return new Mod[]
                    {
                        new RushModNoFail(),
                        new MultiMod(new RushModHalfTime(), new RushModDaycore())
                    };

                case ModType.DifficultyIncrease:
                    return new Mod[]
                    {
                        new MultiMod(new RushModSuddenDeath(), new RushModPerfect()),
                        new MultiMod(new RushModDoubleTime(), new RushModNightcore()),
                        new RushModFlashlight()
                    };

                case ModType.Automation:
                    return new[]
                    {
                        new MultiMod(new RushModAutoplay(), new RushModCinema())
                    };

                case ModType.Fun:
                    return new[]
                    {
                        new MultiMod(new ModWindUp(), new ModWindDown())
                    };

                default:
                    return Array.Empty<Mod>();
            }
        }

        public override IEnumerable<KeyBinding> GetDefaultKeyBindings(int variant = 0) => new[]
        {
            new KeyBinding(InputKey.J, RushAction.GroundPrimary),
            new KeyBinding(InputKey.K, RushAction.GroundSecondary),
            new KeyBinding(InputKey.L, RushAction.GroundTertiary),
            new KeyBinding(InputKey.Semicolon, RushAction.GroundQuaternary),

            new KeyBinding(InputKey.F, RushAction.AirPrimary),
            new KeyBinding(InputKey.D, RushAction.AirSecondary),
            new KeyBinding(InputKey.S, RushAction.AirTertiary),
            new KeyBinding(InputKey.A, RushAction.AirQuaternary),

            new KeyBinding(InputKey.MouseRight, RushAction.GroundPrimary),
            new KeyBinding(InputKey.MouseLeft, RushAction.AirPrimary),

            new KeyBinding(InputKey.Space, RushAction.Fever),
        };

        public override IConvertibleReplayFrame CreateConvertibleReplayFrame() => new RushReplayFrame();

        public override Drawable CreateIcon() => new RushIcon();

        protected override IEnumerable<HitResult> GetValidHitResults()
        {
            return new[]
            {
                HitResult.Great,
                HitResult.Good,
                HitResult.SmallBonus,
                HitResult.LargeBonus
            };
        }

        public override string GetDisplayNameForHitResult(HitResult result) => result switch
        {
            HitResult.SmallBonus => "Heart bonus",
            HitResult.LargeBonus => "Fever bonus",
            _ => base.GetDisplayNameForHitResult(result)
        };

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
                };

                if (!RushRuleset.IsDevelopmentBuild)
                {
                    AddInternal(new SpriteIcon
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Scale = new Vector2(0.6f),
                        Icon = FontAwesome.Solid.Running,
                    });
                }
                else
                {
                    AddRangeInternal(new[]
                    {
                        new SpriteIcon
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Scale = new Vector2(0.4f),
                            Icon = FontAwesome.Solid.Running,
                            Margin = new MarginPadding { Bottom = 0.5f, Right = 0.5f },
                        },
                        new SpriteIcon
                        {
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            Scale = new Vector2(0.4f),
                            Icon = FontAwesome.Solid.Wrench,
                            Margin = new MarginPadding { Top = 0.5f, Left = 0.5f },
                        }
                    });
                }
            }
        }
    }
}
