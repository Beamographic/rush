// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Objects.Drawables;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableDualHitPart : DrawableLanedHit<DualHitPart>
    {
        private const double rotation_time = 1000;

        private readonly SpriteIcon gearSpriteIcon;
        private readonly Box background;
        private readonly Triangles triangles;

        public DrawableDualHitPart(DualHitPart hitObject)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE);

            Content.AddRange(new Drawable[]
            {
                gearSpriteIcon = new SpriteIcon
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Icon = FontAwesome.Solid.Cog
                },
                new CircularContainer
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE),
                    Scale = new Vector2(0.7f),
                    BorderThickness = RushPlayfield.HIT_TARGET_SIZE * 0.1f,
                    BorderColour = Color4.White,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        background = new Box { RelativeSizeAxes = Axes.Both },
                        triangles = new Triangles { RelativeSizeAxes = Axes.Both }
                    }
                },
            });

            AccentColour.ValueChanged += evt => updateColours(evt.NewValue);
        }

        private void updateColours(Color4 colour)
        {
            background.Colour = colour.Darken(0.5f);
            triangles.Colour = colour;
            triangles.Alpha = 0.8f;
            gearSpriteIcon.Colour = colour.Lighten(0.5f);
        }

        public new bool UpdateResult(bool userTriggered) => base.UpdateResult(userTriggered);

        public override bool OnPressed(RushAction action) => false;

        public override void OnReleased(RushAction action)
        {
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            const float animation_time = 300f;

            switch (state)
            {
                case ArmedState.Idle:
                    UnproxyContent();
                    break;

                case ArmedState.Miss:
                    this.FadeOut(animation_time);
                    break;

                case ArmedState.Hit:
                    ProxyContent();

                    float travelY = 400f * (HitObject.Lane == LanedHitLane.Air ? -1 : 1);

                    this.MoveToY(travelY, animation_time, Easing.Out);
                    this.FadeOut(animation_time);

                    break;
            }
        }

        protected override void Update()
        {
            base.Update();

            gearSpriteIcon.Rotation = (float)(Time.Current % rotation_time / rotation_time) * 360f;
        }
    }
}
