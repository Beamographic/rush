// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Scoring;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableSawblade : DrawableLanedHit<Sawblade>
    {
        private readonly Saw saw;

        [Resolved]
        private RushPlayfield playfield { get; set; }

        // Sawblade uses the reverse lane colour to indicate which key the player should tap to avoid it
        public override Color4 LaneAccentColour => HitObject.Lane == LanedHitLane.Ground ? AIR_ACCENT_COLOUR : GROUND_ACCENT_COLOUR;

        protected override bool ExpireOnHit => false;
        protected override bool ExpireOnMiss => false;

        public DrawableSawblade(Sawblade hitObject)
            : base(hitObject)
        {
            Size = new Vector2(RushPlayfield.HIT_TARGET_SIZE * 2f);

            Content.AddRange(new[]
            {
                new Container
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Size = new Vector2(0.8f),
                    Masking = hitObject.Lane == LanedHitLane.Ground,
                    Child = saw = new Saw
                    {
                        Origin = Anchor.Centre,
                        Anchor = hitObject.Lane == LanedHitLane.Ground ? Anchor.BottomCentre : Anchor.TopCentre,
                        RelativeSizeAxes = Axes.Both,
                        Size = new Vector2(0.8f)
                    }
                }
            });

            AccentColour.ValueChanged += _ => updateDrawables();
        }

        private void updateDrawables()
        {
            saw.UpdateColour(AccentColour.Value);
        }

        // Sawblade doesn't handle user presses at all.
        public override bool OnPressed(RushAction action) => false;

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            // sawblades can't be user triggered, and will not hurt the player in the leading hit windows
            if (userTriggered || timeOffset < 0 || AllJudged)
                return;

            switch (HitObject.HitWindows.ResultFor(timeOffset))
            {
                case HitResult.None:
                    // if we've reached the trailing "none", we successfully dodged the sawblade
                    ApplyResult(r => r.Type = HitResult.Perfect);
                    break;

                case HitResult.Miss:
                    // sawblades only hurt the player if they collide within the trailing "miss" hit window
                    if (playfield.PlayerSprite.CollidesWith(HitObject))
                        ApplyResult(r => r.Type = HitResult.Miss);

                    break;
            }
        }

        protected class Saw : CompositeDrawable
        {
            private const double rotation_time = 1000;

            private readonly SpriteIcon outerSawIcon;
            private readonly SpriteIcon innerSawIcon;
            private readonly Box backgroundBox;
            private readonly Triangles triangles;

            public Saw()
            {
                InternalChildren = new Drawable[]
                {
                    outerSawIcon = new SpriteIcon
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Icon = FontAwesome.Solid.Sun,
                        Colour = Color4.White,
                        RelativeSizeAxes = Axes.Both,
                        Scale = new Vector2(1.1f)
                    },
                    innerSawIcon = new SpriteIcon
                    {
                        Origin = Anchor.Centre,
                        Anchor = Anchor.Centre,
                        Icon = FontAwesome.Solid.Sun,
                        RelativeSizeAxes = Axes.Both,
                    },
                    new CircularContainer
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        RelativeSizeAxes = Axes.Both,
                        BorderThickness = DrawableNoteSheet.NOTE_SHEET_SIZE * 0.1f,
                        BorderColour = Color4.White,
                        Masking = true,
                        Size = new Vector2(0.75f),
                        Children = new Drawable[]
                        {
                            backgroundBox = new Box { RelativeSizeAxes = Axes.Both },
                            triangles = new Triangles { RelativeSizeAxes = Axes.Both }
                        }
                    }
                };
            }

            public void UpdateColour(Color4 colour)
            {
                backgroundBox.Colour = colour.Darken(0.5f);
                triangles.Colour = colour;
                triangles.Alpha = 0.8f;
                innerSawIcon.Colour = colour.Lighten(0.5f);
            }

            protected override void Update()
            {
                base.Update();

                innerSawIcon.Rotation = outerSawIcon.Rotation = (float)(Time.Current % rotation_time / rotation_time) * 360f;
            }
        }
    }
}
