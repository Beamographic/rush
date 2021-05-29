// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI.Fever
{
    public class FeverBar : CircularContainer
    {
        public readonly IBindable<float> FeverProgress;
        public readonly IBindable<bool> FeverActivated;

        private readonly Box progressBar;

        public FeverBar(FeverTracker tracker)
        {
            FeverProgress = tracker.FeverProgress.GetBoundCopy();
            FeverActivated = tracker.FeverActivated.GetBoundCopy();

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
                            Font = OsuFont.Numeric.With(size: 20),
                            UseFullGlyphHeight = false
                        },
                        new FeverRollingCounter
                        {
                            Anchor = Anchor.CentreRight,
                            Origin = Anchor.CentreRight,
                            Colour = Color4.White,
                            Current = { BindTarget = FeverProgress }
                        }
                    }
                }
            };

            FeverProgress.BindValueChanged(v => updateProgressBar(v.NewValue));
            FeverActivated.ValueChanged += updateFeverState;
        }

        private void updateProgressBar(float progress)
        {
            if (!FeverActivated.Value)
            {
                if (progress == 1)
                    FadeEdgeEffectTo(0.5f, 100);
                else
                    FadeEdgeEffectTo(0, 100);
            }
            progressBar.ResizeWidthTo(progress, 100);
        }
        private void updateFeverState(ValueChangedEvent<bool> valueChanged)
        {
            FinishTransforms(true);
            if (valueChanged.NewValue)
            {
                FadeEdgeEffectTo(1, 100);
                progressBar.FadeColour(Color4.Red, 100);
            }
            else
            {
                progressBar.FadeColour(Color4.DeepPink, 200);
            }
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
                    UseFullGlyphHeight = false,
                };
            }

            protected override string FormatCount(float count)
            {
                return (count * 100).ToString("0\\%");
            }
        }
    }
}