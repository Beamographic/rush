// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Rulesets.Dash.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Screens.Multi;
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

        private readonly Drawable bodyPiece;

        public double? HoldStartTime { get; private set; }

        public bool HasBroken { get; private set; }

        public DrawableNoteSheet(NoteSheet hitObject)
            : base(hitObject)
        {
            Height = DashPlayfield.HIT_TARGET_SIZE;

            AddRangeInternal(new[]
            {
                bodyPiece = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Color4.Blue,
                    Alpha = 0.5f
                },
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
                        Origin = Anchor.CentreLeft,
                        AccentColour = { BindTarget = AccentColour }
                    };

                case NoteSheetTail _:
                    return new DrawableNoteSheetTail(this)
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        AccentColour = { BindTarget = AccentColour }
                    };
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            Origin = bodyPiece.Origin = bodyPiece.Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (Tail.AllJudged)
                ApplyResult(r => r.Type = HitResult.Perfect);

            if (Tail.Result.Type == HitResult.Miss)
            {
                HasBroken = true;
                updateColours();
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
            updateColours();
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
                Alpha = 0.5f;
            }
        }

        private void endHold()
        {
            HoldStartTime = null;
            isHitting.Value = false;
        }

        private void updateColours()
        {
            if (HasBroken)
                bodyPiece.Alpha = 0.25f;
            else if (IsHitting.Value)
                bodyPiece.Alpha = 0.9f;
            else
                bodyPiece.Alpha = 0.5f;
        }
    }
}
