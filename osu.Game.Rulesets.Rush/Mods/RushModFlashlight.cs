// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Layout;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Rush.Mods
{
    public class RushModFlashlight : ModFlashlight<RushHitObject>
    {
        public override double ScoreMultiplier => 1.12;

        private const float default_flashlight_size = 330;

        public override Flashlight CreateFlashlight() => new RushFlashlight(playfield);

        private RushPlayfield playfield;

        public override void ApplyToDrawableRuleset(DrawableRuleset<RushHitObject> drawableRuleset)
        {
            playfield = (RushPlayfield)drawableRuleset.Playfield;
            base.ApplyToDrawableRuleset(drawableRuleset);
        }

        private class RushFlashlight : Flashlight
        {
            private readonly LayoutValue flashlightProperties = new LayoutValue(Invalidation.DrawSize);
            private readonly RushPlayfield rushPlayfield;

            public RushFlashlight(RushPlayfield rushPlayfield)
            {
                this.rushPlayfield = rushPlayfield;
                FlashlightSize = new Vector2(0, getSizeFor(0));

                AddLayout(flashlightProperties);
            }

            private float getSizeFor(int combo)
            {
                if (combo > 200)
                    return default_flashlight_size * 0.8f;
                else if (combo > 100)
                    return default_flashlight_size * 0.9f;
                else
                    return default_flashlight_size;
            }

            protected override void OnComboChange(ValueChangedEvent<int> e)
            {
                this.TransformTo(nameof(FlashlightSize), new Vector2(0, getSizeFor(e.NewValue)), FLASHLIGHT_FADE_DURATION);
            }

            protected override string FragmentShader => "CircularFlashlight";

            protected override void Update()
            {
                base.Update();

                if (!flashlightProperties.IsValid)
                {
                    var flashlightPosition = rushPlayfield.OverPlayerEffectsContainer.ToSpaceOfOtherDrawable(rushPlayfield.OverPlayerEffectsContainer.OriginPosition, this);
                    flashlightPosition.X += RushPlayfield.HIT_TARGET_OFFSET;
                    FlashlightPosition = flashlightPosition;
                    flashlightProperties.Validate();
                }
            }
        }
    }
}
