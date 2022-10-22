// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Input;
using osu.Game.Beatmaps;
using osu.Game.Configuration;
using osu.Game.Input.Handlers;
using osu.Game.Replays;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Configuration;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Replays;
using osu.Game.Rulesets.Rush.UI.Fever;
using osu.Game.Rulesets.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Scoring;

namespace osu.Game.Rulesets.Rush.UI
{
    [Cached]
    public class DrawableRushRuleset : DrawableScrollingRuleset<RushHitObject>
    {
        private FeverProcessor feverProcessor;

        protected new RushRulesetConfigManager Config => (RushRulesetConfigManager)base.Config;

        public new RushPlayfield Playfield => (RushPlayfield)base.Playfield;

        public new RushInputManager KeyBindingInputManager => (RushInputManager)base.KeyBindingInputManager;

        public FeverActivationMode FeverActivationMode => KeyBindingInputManager.ReplayInputHandler?.FeverActivationMode ?? feverActivationModeSetting.Value;

        protected override bool UserScrollSpeedAdjustment => true;

        protected override ScrollVisualisationMethod VisualisationMethod => ScrollVisualisationMethod.Constant;

        public DrawableRushRuleset(RushRuleset ruleset, IBeatmap beatmap, IReadOnlyList<Mod> mods = null)
            : base(ruleset, beatmap, mods)
        {
            Direction.Value = ScrollingDirection.Left;
            TimeRange.Value = 800;
        }

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            var dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(feverProcessor = new FeverProcessor());

            NewResult += feverProcessor.ApplyResult;
            RevertResult += feverProcessor.RevertResult;

            return dependencies;
        }

        private readonly Bindable<FeverActivationMode> feverActivationModeSetting = new Bindable<FeverActivationMode>();

        [BackgroundDependencyLoader]
        private void load()
        {
            Config?.BindWith(RushRulesetSettings.FeverActivationMode, feverActivationModeSetting);

            FrameStableComponents.Add(feverProcessor);
        }

        public bool PlayerCollidesWith(HitObject hitObject) => Playfield.PlayerSprite.CollidesWith(hitObject);

        protected override Playfield CreatePlayfield() => new RushPlayfield();

        protected override ReplayInputHandler CreateReplayInputHandler(Replay replay) => new RushFramedReplayInputHandler(replay);

        protected override ReplayRecorder CreateReplayRecorder(Score score) => new RushReplayRecorder(score);

        public override DrawableHitObject<RushHitObject> CreateDrawableRepresentation(RushHitObject h) => null;

        protected override PassThroughInputManager CreateInputManager() => new RushInputManager(Ruleset?.RulesetInfo);
    }
}
