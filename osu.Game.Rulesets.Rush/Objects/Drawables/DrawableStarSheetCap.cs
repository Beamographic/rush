// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public abstract class DrawableStarSheetCap<TObject> : DrawableLanedHit<TObject>
        where TObject : LanedHit
    {
        protected abstract RushSkinComponents Component { get; }

        private readonly Drawable capPiece;

        protected readonly DrawableStarSheet StarSheet;

        [Resolved]
        private RushPlayfield playfield { get; set; }

        public override bool DisplayExplosion => true;

        protected DrawableStarSheetCap(DrawableStarSheet starSheet, TObject hitObject)
            : base(hitObject)
        {
            StarSheet = starSheet;
            Size = new Vector2(DrawableStarSheet.NOTE_SHEET_SIZE * 1.1f);
            Origin = Anchor.Centre;

            Content.Child = capPiece = new SkinnableDrawable(new RushSkinComponent(Component), _ => new StarSheetCapStarPiece())
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            };
        }

        public override Drawable CreateHitExplosion() => new StarSheetHitExplosion(this);

        protected override void UpdateStartTimeStateTransforms()
        {
            Scale = Vector2.One;
            Alpha = 1f;
        }

        protected override void UpdateHitStateTransforms(ArmedState state)
        {
            base.UpdateHitStateTransforms(state);

            if (state == ArmedState.Hit)
                Hide();
        }

        public bool TriggerResult() => UpdateResult(true);

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
        }

        public override bool OnPressed(RushAction action) => false; // Handled by the star sheet object itself.

        public override void OnReleased(RushAction action)
        {
        }

        private class StarSheetHitExplosion : CompositeDrawable
        {
            private readonly StarSheetCapStarPiece explosionStar;
            private readonly Circle flashCircle;

            public StarSheetHitExplosion(DrawableStarSheetCap<TObject> drawableStarSheet)
            {
                Anchor = drawableStarSheet.LaneAnchor;
                Origin = Anchor.Centre;
                Size = drawableStarSheet.Size;

                InternalChildren = new Drawable[]
                {
                    explosionStar = new StarSheetCapStarPiece(),
                    flashCircle = new Circle
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = 0.4f,
                        RelativeSizeAxes = Axes.Both,
                        Scale = new Vector2(0.5f),
                        Colour = drawableStarSheet.LaneAccentColour.Lighten(0.5f)
                    }
                };
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();

                explosionStar.ScaleTo(2f, RushPlayfield.HIT_EXPLOSION_DURATION)
                             .FadeOutFromOne(RushPlayfield.HIT_EXPLOSION_DURATION)
                             .Expire(true);

                flashCircle.ScaleTo(4f, RushPlayfield.HIT_EXPLOSION_DURATION / 2)
                           .Then()
                           .ScaleTo(0.5f, RushPlayfield.HIT_EXPLOSION_DURATION / 2)
                           .FadeOut(RushPlayfield.HIT_EXPLOSION_DURATION / 2)
                           .Expire(true);

                // TODO: very low priority for now, but this shouldn't stay as-is in every similar composite.
                this.Delay(InternalChildren.Max(d => d.LatestTransformEndTime)).Expire(true);
            }
        }
    }
}
