// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Screens.Play;
using osu.Game.Tests.Visual;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Tests.Visual
{
    public class TestSceneTouchInputHandling : OsuManualInputManagerTestScene
    {
        protected override Ruleset CreateRuleset() => new RushRuleset();

        private static readonly TouchSource[] touch_sources = (TouchSource[])Enum.GetValues(typeof(TouchSource));

        private TouchRegion airRegion;
        private TouchRegion groundRegion;
        private TouchRegion feverRegion;

        private KeyCounterDisplay keyCounters;
        private RushInputManager rushInputManager;

        [BackgroundDependencyLoader]
        private void load()
        {
            Children = new Drawable[]
            {
                airRegion = new TouchRegion
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Size = new Vector2(1,0.5f),

                    Action = RushActionTarget.Air,
                    Colour = Color4.Aqua,
                    Alpha = 0.8f,
                },
                groundRegion = new TouchRegion
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                    Size = new Vector2(1,0.5f),

                    Action = RushActionTarget.Ground,
                    Colour = Color4.Red,
                    Alpha = 0.8f,
                },
                feverRegion = new TouchRegion
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(0.25f, 0.25f),

                    Action = RushActionTarget.Fever,
                    Colour = Color4.Purple,
                    Alpha = 0.8f,
                },
                keyCounters = new KeyCounterDisplay
                {
                    Origin = Anchor.BottomRight,
                    Anchor = Anchor.BottomRight,
                }
            };

            rushInputManager = new RushInputManager(Ruleset.Value);
            rushInputManager.Attach(keyCounters);

            var tmpChild = InputManager.Child;
            InputManager.Clear(false);

            InputManager.Child = rushInputManager.WithChild(tmpChild);
        }

        [Test]
        public void TestPreferEarliestFreeAction()
        {

            AddStep("Touch ground area (1)", () => touchDrawable(TouchSource.Touch1, groundRegion));
            AddStep("Touch ground area (2)", () => touchDrawable(TouchSource.Touch2, groundRegion));
            AddAssert("B1 on, B2 on", () => inputStateOnlyContains(RushAction.GroundPrimary, RushAction.GroundSecondary));

            AddStep("Release ground area (1)", () => endTouch(TouchSource.Touch1));
            AddAssert("B1 off, B2 on", () => inputStateOnlyContains(RushAction.GroundSecondary));

            AddStep("Touch ground area (3)", () => touchDrawable(TouchSource.Touch3, groundRegion));
            AddAssert("B1 on, B2 on", () => inputStateOnlyContains(RushAction.GroundPrimary, RushAction.GroundSecondary));

            AddStep("Touch ground area (4)", () => touchDrawable(TouchSource.Touch4, groundRegion));
            AddAssert("B1 on, B2 on, B3 on", () => inputStateOnlyContains(RushAction.GroundPrimary, RushAction.GroundSecondary, RushAction.GroundTertiary));

            AddStep("Release ground area (2)", () => endTouch(TouchSource.Touch2));
            AddAssert("B1 on, B2 off, B3 on", () => inputStateOnlyContains(RushAction.GroundPrimary, RushAction.GroundTertiary));

            AddStep("Touch ground area (5)", () => touchDrawable(TouchSource.Touch5, groundRegion));
            AddAssert("B1 on, B2 on, B3 on", () => inputStateOnlyContains(RushAction.GroundPrimary, RushAction.GroundSecondary, RushAction.GroundTertiary));
        }

        [Test]
        public void TestIgnoreExcessiveTouches()
        {
            AddStep("Touch ground area (1)", () => touchDrawable(TouchSource.Touch1, groundRegion));
            AddStep("Touch ground area (2)", () => touchDrawable(TouchSource.Touch2, groundRegion));
            AddStep("Touch ground area (3)", () => touchDrawable(TouchSource.Touch3, groundRegion));
            AddStep("Touch ground area (4)", () => touchDrawable(TouchSource.Touch4, groundRegion));
            AddAssert("All ground actions on", () => inputStateOnlyContains(RushAction.GroundPrimary, RushAction.GroundSecondary, RushAction.GroundTertiary, RushAction.GroundQuaternary));

            AddStep("Touch ground area (5)", () => touchDrawable(TouchSource.Touch5, groundRegion));
            AddAssert("Only ground actions on", () => inputStateOnlyContains(RushAction.GroundPrimary, RushAction.GroundSecondary, RushAction.GroundTertiary, RushAction.GroundQuaternary));
        }

        [Test]
        public void TestTouchConversionBlocking()
        {
            AddStep("Touch Fever region", () => touchDrawableWithOffset(TouchSource.Touch1, feverRegion, new Vector2(0, 10)));

            AddAssert("Only fever action on", () => inputStateOnlyContains(RushAction.Fever));
        }

        [Test]
        public void TestReleaseAfterSwitchingRegions()
        {
            AddStep("Touch ground region", () => touchDrawable(TouchSource.Touch1, groundRegion));
            AddStep("Move to air region", () => moveTouchToDrawable(TouchSource.Touch1, airRegion));
            AddAssert("Ground action still held", () => inputStateOnlyContains(RushAction.GroundPrimary));
            AddStep("Release touch", () => endTouch(TouchSource.Touch1));
            AddAssert("Ground action released", () => inputStateOnlyContains());
        }

        [SetUp]
        public void ReleaseAllTouches()
        {
            foreach (var source in touch_sources)
                InputManager.EndTouch(new Touch(source, Vector2.Zero));
        }

        private bool inputStateOnlyContains(params RushAction[] actions) => rushInputManager.PressedActions.ToHashSet().SetEquals(actions);

        private void touchDrawable(TouchSource source, Drawable drawable) => InputManager.BeginTouch(new Touch(source, drawable.ScreenSpaceDrawQuad.Centre));
        private void touchDrawableWithOffset(TouchSource source, Drawable drawable, Vector2 offset) => InputManager.BeginTouch(new Touch(source, drawable.ScreenSpaceDrawQuad.Centre + offset));
        private void moveTouchToDrawable(TouchSource source, Drawable drawable) => InputManager.MoveTouchTo(new Touch(source, drawable.ScreenSpaceDrawQuad.Centre));
        private void endTouch(TouchSource source) => InputManager.EndTouch(new Touch(source, Vector2.Zero));

        private class TouchRegion : Box, IKeyBindingTouchHandler
        {
            public override bool HandlePositionalInput => true;

            public RushActionTarget Action;

            public RushActionTarget ActionTargetForTouchPosition(Vector2 screenSpaceTouchPos) => Action;
        }
    }
}
