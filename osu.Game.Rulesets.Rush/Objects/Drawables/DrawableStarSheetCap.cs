// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;
using osuTK;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public abstract class DrawableStarSheetCap<TObject> : DrawableLanedHit<TObject>
        where TObject : LanedHit
    {
        protected abstract RushSkinComponents Component { get; }

        protected DrawableStarSheet StarSheet => (DrawableStarSheet)ParentHitObject;

        protected abstract Anchor CapAnchor { get; }

        public override bool DisplayExplosion => true;

        protected DrawableStarSheetCap(TObject hitObject)
            : base(hitObject)
        {
            Size = new Vector2(DrawableStarSheet.NOTE_SHEET_SIZE * 1.1f);
            Origin = Anchor.Centre;

            AddInternal(new SkinnableDrawable(new RushSkinComponent(Component), _ => new StarSheetCapStarPiece())
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                RelativeSizeAxes = Axes.Both,
            });
        }

        protected override void OnApply()
        {
            base.OnApply();

            Anchor = CapAnchor;
            AccentColour.BindTo(StarSheet.AccentColour);
        }

        protected override void OnFree()
        {
            base.OnFree();
            AccentColour.UnbindFrom(StarSheet.AccentColour);
        }

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

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) => Anchor = CapAnchor;

        public override bool OnPressed(RushAction action) => false; // Handled by the star sheet object itself.

        public override void OnReleased(RushAction action)
        {
        }
    }
}
