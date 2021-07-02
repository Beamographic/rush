// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Rush.Input;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI.Fever
{
    public class FeverBar : CircularContainer, IKeyBindingTouchHandler
    {
        public override bool HandlePositionalInput => true;

        private Box progressBar;

        public override bool RemoveCompletedTransforms => true;

        public FeverBar()
        {
            Y = 150;
            Anchor = Anchor.BottomCentre;
            Origin = Anchor.TopCentre;
            RelativeSizeAxes = Axes.X;
            Size = new Vector2(0.5f, 50);
            Masking = true;
            BorderColour = Color4.Violet;
            BorderThickness = 5;
            EdgeEffect = new EdgeEffectParameters
            {
                Colour = Color4.DeepPink.Opacity(0),
                Type = EdgeEffectType.Glow,
                Radius = 20,
            };
        }

        private IBindable<bool> inFeverMode;
        private IBindable<float> feverProgress;

        [BackgroundDependencyLoader]
        private void load(FeverProcessor processor)
        {
            feverProgress = processor.FeverProgress.GetBoundCopy();
            inFeverMode = processor.InFeverMode.GetBoundCopy();

            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Purple,
                },
                progressBar = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.DeepPink,
                    Size = new Vector2(0, 1),
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(15),
                    Children = new Drawable[]
                    {
                        new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = "FEVER",
                            Colour = Color4.White,
                            Font = OsuFont.Numeric.With(size: 20),
                            UseFullGlyphHeight = false
                        },
                        new FeverRollingCounter
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Colour = Color4.White,
                            Current = { BindTarget = feverProgress }
                        }
                    }
                }
            };

            feverProgress.ValueChanged += updateProgressBar;
            inFeverMode.ValueChanged += updateFeverState;
        }

        private void updateProgressBar(ValueChangedEvent<float> valueChanged)
        {
            if (!inFeverMode.Value)
            {
                if (valueChanged.NewValue >= 1 && valueChanged.OldValue < 1)
                    FadeEdgeEffectTo(0.5f, 100);
                else if (valueChanged.NewValue < 1 && valueChanged.OldValue >= 1)
                    FadeEdgeEffectTo(0f); // Just to support rewinds
            }

            progressBar.ResizeWidthTo(Math.Min(1, valueChanged.NewValue), 100);

            if (Clock.Rate < 0)
                FinishTransforms(true); // Force the animations to finish immediately when rewinding
        }

        private void updateFeverState(ValueChangedEvent<bool> valueChanged)
        {
            if (valueChanged.NewValue)
            {
                FadeEdgeEffectTo(Color4.Red, 100);
                progressBar.FadeColour(Color4.Red, 100);
            }
            else
            {
                FadeEdgeEffectTo(Color4.DeepPink.Opacity(0), 200);
                progressBar.FadeColour(Color4.DeepPink, 200);
            }

            if (Clock.Rate < 0)
                FinishTransforms(true); // Force the animations to finish immediately when rewinding
        }

        private RushInputManager rushActionInputManager;
        internal RushInputManager RushActionInputManager => rushActionInputManager ??= GetContainingInputManager() as RushInputManager;

        public RushActionTarget ActionTargetForTouchPosition(Vector2 screenSpaceTouchPos) => RushActionTarget.Fever;

        private class FeverRollingCounter : RollingCounter<float>
        {
            protected override double RollingDuration => 100;

            public FeverRollingCounter()
            {
                Anchor = Anchor.Centre;
                Origin = Anchor.Centre;
            }

            protected override OsuSpriteText CreateSpriteText()
            {
                return new OsuSpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Font = OsuFont.Numeric.With(size: 20),
                    UseFullGlyphHeight = false,
                };
            }

            protected override string FormatCount(float count)
            {
                return Math.Floor(Math.Min(count, 1) * 100).ToString("0\\%");
            }
        }
    }
}
