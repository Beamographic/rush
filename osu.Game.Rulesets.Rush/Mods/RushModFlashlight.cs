// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Layout;
using osu.Game.Configuration;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI;
using osuTK;

namespace osu.Game.Rulesets.Rush.Mods
{
    public partial class RushModFlashlight : ModFlashlight<RushHitObject>
    {
        public override double ScoreMultiplier => 1.12;

        public override float DefaultFlashlightSize => 330;

        [SettingSource("Flashlight size", "Multiplier applied to the default flashlight size.")]
        public override BindableFloat SizeMultiplier { get; } = new BindableFloat
        {
            MinValue = 0.5f,
            MaxValue = 1.5f,
            Default = 1f,
            Value = 1f,
            Precision = 0.1f
        };

        [SettingSource("Change size based on combo", "Decrease the flashlight size as combo increases.")]
        public override BindableBool ComboBasedSize { get; } = new BindableBool
        {
            Default = true,
            Value = true
        };

        protected override Flashlight CreateFlashlight() => new RushFlashlight(this, playfield);

        private RushPlayfield playfield;

        public override void ApplyToDrawableRuleset(DrawableRuleset<RushHitObject> drawableRuleset)
        {
            playfield = (RushPlayfield)drawableRuleset.Playfield;
            base.ApplyToDrawableRuleset(drawableRuleset);
        }

        private partial class RushFlashlight : Flashlight
        {
            private readonly LayoutValue flashlightProperties = new LayoutValue(Invalidation.DrawSize);

            private readonly RushPlayfield rushPlayfield;

            public RushFlashlight(ModFlashlight modFlashlight, RushPlayfield rushPlayfield) : base(modFlashlight)
            {
                this.rushPlayfield = rushPlayfield;
                FlashlightSize = new Vector2(0, GetSize());

                AddLayout(flashlightProperties);
            }

            protected override void UpdateFlashlightSize(float size)
            {
                this.TransformTo(nameof(FlashlightSize), new Vector2(0, size), FLASHLIGHT_FADE_DURATION);
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
