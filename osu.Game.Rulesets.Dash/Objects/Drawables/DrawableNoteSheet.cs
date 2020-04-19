// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics.Backgrounds;
using osu.Game.Rulesets.Dash.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Dash.Objects.Drawables
{
    public class DrawableNoteSheet : DrawableLanedHit<NoteSheet>
    {
        public IBindable<bool> IsHitting => isHitting;

        private readonly Bindable<bool> isHitting = new Bindable<bool>();

        public DrawableNoteSheetHead Head => headContainer.Child;
        public DrawableNoteSheetTail Tail => tailContainer.Child;

        private readonly Container<DrawableNoteSheetHead> headContainer;
        private readonly Container<DrawableNoteSheetTail> tailContainer;

        private readonly NoteSheetBody noteSheetBody;

        public double? HoldStartTime { get; private set; }

        public bool HasBroken { get; private set; }

        public DrawableNoteSheet(NoteSheet hitObject)
            : base(hitObject)
        {
            Height = DashPlayfield.HIT_TARGET_SIZE;

            AddRangeInternal(new Drawable[]
            {
                noteSheetBody = new NoteSheetBody(this) { RelativeSizeAxes = Axes.Both, Alpha = 0.75f },
                headContainer = new Container<DrawableNoteSheetHead> { RelativeSizeAxes = Axes.Both },
                tailContainer = new Container<DrawableNoteSheetTail> { RelativeSizeAxes = Axes.Both }
            });
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableNoteSheetHead head:
                    headContainer.Child = head;
                    break;

                case DrawableNoteSheetTail tail:
                    tailContainer.Child = tail;
                    break;
            }
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            headContainer.Clear();
            tailContainer.Clear();
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case NoteSheetHead _:
                    return new DrawableNoteSheetHead(this)
                    {
                        Anchor = Anchor.CentreLeft,
                        AccentColour = { BindTarget = AccentColour }
                    };

                case NoteSheetTail _:
                    return new DrawableNoteSheetTail(this)
                    {
                        Anchor = Anchor.CentreLeft,
                        AccentColour = { BindTarget = AccentColour }
                    };
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            Origin = noteSheetBody.Origin = noteSheetBody.Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (Tail.AllJudged)
                ApplyResult(r => r.Type = HitResult.Perfect);

            if (Tail.Result.Type == HitResult.Miss)
            {
                HasBroken = true;
                noteSheetBody.UpdateState();
                Alpha = 0.25f;
            }
        }

        public override bool OnPressed(DashAction action)
        {
            if (AllJudged)
                return false;

            if (!LaneMatchesAction(action))
                return false;

            beginHoldAt(Time.Current - Head.HitObject.StartTime);
            Head.UpdateResult();

            return true;
        }

        private void beginHoldAt(double timeOffset)
        {
            if (timeOffset < -Head.HitObject.HitWindows.WindowFor(HitResult.Miss))
                return;

            HoldStartTime = Time.Current;
            isHitting.Value = true;
            noteSheetBody.UpdateState();

            Alpha = HasBroken ? 0.25f : 1f;
        }

        public override void OnReleased(DashAction action)
        {
            if (AllJudged)
                return;

            if (!LaneMatchesAction(action))
                return;

            // Make sure a hold was started
            if (HoldStartTime == null)
                return;

            Tail.UpdateResult();
            endHold();

            // If the key has been released too early, the user should not receive full score for the release
            if (!Tail.IsHit)
            {
                HasBroken = true;
                Alpha = 0.25f;
            }
        }

        private void endHold()
        {
            HoldStartTime = null;
            isHitting.Value = false;
        }

        private class NoteSheetBody : CompositeDrawable
        {
            private const float border_height = 5;

            private readonly DrawableNoteSheet noteSheet;
            private readonly Box backgroundBox;
            private readonly Triangles triangles;
            private readonly Box topBox;
            private readonly Box bottomBox;

            public NoteSheetBody(DrawableNoteSheet noteSheet)
            {
                this.noteSheet = noteSheet;

                Masking = true;

                AddRangeInternal(new Drawable[]
                {
                    backgroundBox = new Box { RelativeSizeAxes = Axes.Both },
                    triangles = new Triangles { RelativeSizeAxes = Axes.Both },
                    topBox = new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = border_height,
                        Origin = Anchor.TopCentre,
                        Anchor = Anchor.TopCentre,
                        Colour = Color4.White
                    },
                    bottomBox = new Box
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = border_height,
                        Origin = Anchor.BottomCentre,
                        Anchor = Anchor.BottomCentre,
                        Colour = Color4.White
                    }
                });

                UpdateState();
            }

            public void UpdateState()
            {
                backgroundBox.Colour = noteSheet.HasBroken ? Color4.Gray : new Color4(0f, 0f, 0.2f, 1f);
                triangles.Colour = noteSheet.HasBroken ? Color4.Gray : Color4.Blue;
            }
        }
    }
}
