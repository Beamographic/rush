// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Dash.UI;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheetTail : DrawableLanedHit<NoteSheetTail>
    {
        private const double release_window_lenience = 1.5;
        private const double rotation_time = 1000;

        private readonly DrawableNoteSheet noteSheet;

        private readonly Sprite sprite;

        public DrawableNoteSheetTail(DrawableNoteSheet noteSheet)
            : base(noteSheet.HitObject.Tail)
        {
            this.noteSheet = noteSheet;

            Size = new Vector2(DashPlayfield.HIT_TARGET_SIZE * 2);
            Origin = Anchor.Centre;

            AddInternal(sprite = new Sprite
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                FillMode = FillMode.Fit,
                RelativeSizeAxes = Axes.Both
            });
        }

        public void UpdateResult() => base.UpdateResult(true);

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            Debug.Assert(HitObject.HitWindows != null);

            // Factor in the release lenience
            timeOffset /= release_window_lenience;

            if (!userTriggered)
            {
                if (!HitObject.HitWindows.CanBeHit(timeOffset))
                    ApplyResult(r => r.Type = HitResult.Miss);

                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);
            if (result == HitResult.None)
                return;

            ApplyResult(r =>
            {
                // If the head wasn't hit or the hold note was broken, cap the max score to Meh.
                if (result > HitResult.Meh && (!noteSheet.Head.IsHit || noteSheet.HasBroken))
                    result = HitResult.Meh;

                r.Type = result;
            });
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e) =>
            Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            sprite.Texture = store.Get("star");
        }

        protected override void Update()
        {
            base.Update();

            sprite.Rotation = (float)(Time.Current % rotation_time / rotation_time) * 360f;
        }

        public override bool OnPressed(DashAction action) => false;

        public override void OnReleased(DashAction action)
        {
        }
    }
}
