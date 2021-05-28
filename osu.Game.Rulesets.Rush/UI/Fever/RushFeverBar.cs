// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Rulesets.Judgements;
using osu.Game.Rulesets.Rush.Judgements;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI.Fever
{
    public class FeverBar : CircularContainer, IKeyBindingHandler<RushAction>
    {
        public readonly BindableFloat FeverProgress = new BindableFloat(0);

        private readonly Box progressBar;

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
                    Size = new Vector2(0,1),
                },
                new Container{
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(15),
                    Children = new Drawable[]{
                        new OsuSpriteText
                        {
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            Text = "FEVER",
                            Colour = Color4.White,
                            Font = OsuFont.Numeric.With(size: 20)
                        },
                        new FeverRollingCounter
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Colour = Color4.White,
                            Current = FeverProgress.GetBoundCopy()
                        }
                    }
                }
            };

            FeverProgress.BindValueChanged(v => updateProgressBar(v.NewValue));
        }

        private void updateProgressBar(float progress)
        {
            if (!FeverActivated.Value)
            {
                if (progress == 1)
                    FadeEdgeEffectTo(0.5f, 200);
                else
                    FadeEdgeEffectTo(0, 200);
            }

            progressBar.ResizeWidthTo(progress, 200);
        }

        public Bindable<bool> FeverActivated = new Bindable<bool>();

        public void HandleResult(JudgementResult result)
        {
            if (FeverActivated.Value)
                return;

            FeverProgress.Value = Math.Min(FeverProgress.Value + feverIncreaseFor(result), 1);
        }

        public bool OnPressed(RushAction action)
        {
            if (action != RushAction.Fever)
                return false;

            if (FeverActivated.Value)
                return false;

            if (FeverProgress.Value < 1)
                return false;

            FeverActivated.Value = true;
            FadeEdgeEffectTo(Color4.Red).TransformBindableTo(FeverProgress, 0, 10000).Finally(_ => FeverActivated.Value = false);
            this.Delay(10000).FadeEdgeEffectTo(Color4.DeepPink.Opacity(0));
            return true;
        }

        public void OnReleased(RushAction action) { }

        private float feverIncreaseFor(JudgementResult result)
        {
            if (result.Judgement is RushIgnoreJudgement) return 0;
            return result.Judgement.NumericResultFor(result) / result.Judgement.MaxNumericResult / 50f;
        }

        private class FeverRollingCounter : RollingCounter<float>
        {
            protected override double RollingDuration => 200;

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
                };
            }

            protected override string FormatCount(float count)
            {
                return (count * 100).ToString("0\\%");
            }
        }
    }
}
