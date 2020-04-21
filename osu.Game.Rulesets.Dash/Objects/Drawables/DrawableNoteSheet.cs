// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Graphics;
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
        public const float NOTE_SHEET_SIZE = DashPlayfield.HIT_TARGET_SIZE * 0.75f;

        public IBindable<bool> IsHitting => isHitting;

        private readonly Bindable<bool> isHitting = new Bindable<bool>();

        public DrawableNoteSheetHead Head => headContainer.Child;
        public DrawableNoteSheetTail Tail => tailContainer.Child;

        private readonly Container<NoteSheetBody> bodyContainer;
        private readonly Container<DrawableNoteSheetHead> headContainer;
        private readonly Container<DrawableNoteSheetTail> tailContainer;

        private readonly NoteSheetBody noteSheetBody;

        public double? HoldStartTime { get; private set; }
        public double? HoldEndTime { get; private set; }

        public bool HasBroken { get; private set; }

        [Resolved]
        private IScrollingInfo scrollingInfo { get; set; }

        public DrawableNoteSheet(NoteSheet hitObject)
            : base(hitObject)
        {
            Height = NOTE_SHEET_SIZE;

            Content.AddRange(new Drawable[]
            {
                bodyContainer = new Container<NoteSheetBody>
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    Origin = Anchor.CentreRight,
                    Anchor = Anchor.CentreRight,
                    Child = noteSheetBody = new NoteSheetBody
                    {
                        Origin = Anchor.CentreRight,
                        Anchor = Anchor.CentreRight,
                        RelativeSizeAxes = Axes.Y
                    },
                },
                headContainer = new Container<DrawableNoteSheetHead> { RelativeSizeAxes = Axes.Both },
                tailContainer = new Container<DrawableNoteSheetTail> { RelativeSizeAxes = Axes.Both }
            });

            AccentColour.BindValueChanged(evt => noteSheetBody.UpdateColours(evt.NewValue), true);
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

            Origin = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
            noteSheetBody.Origin = noteSheetBody.Anchor = e.NewValue == ScrollingDirection.Left ? Anchor.CentreRight : Anchor.CentreLeft;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (Tail.AllJudged)
                ApplyResult(r => r.Type = HitResult.Perfect);

            if (Tail.Result.Type == HitResult.Miss)
            {
                HasBroken = true;
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
        }

        public override void OnReleased(DashAction action)
        {
            if (AllJudged)
                return;

            if (!LaneMatchesAction(action))
                return;

            // Make sure a hold was started and not ended
            if (HoldStartTime == null || HoldEndTime != null)
                return;

            Tail.UpdateResult();
            endHold();

            // If the key has been released too early, the user should not receive full score for the release
            if (!Tail.IsHit)
            {
                HasBroken = true;
                ApplyResult(r => r.Type = HitResult.Miss);
            }
        }

        private void endHold()
        {
            HoldEndTime = Time.Current;
            isHitting.Value = false;
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            switch (state)
            {
                case ArmedState.Idle:
                    AccentColour.Value = LaneAccentColour;
                    break;

                case ArmedState.Hit:
                    break;

                case ArmedState.Miss:
                    AccentColour.Value = Color4.Gray;
                    break;
            }
        }

        protected override void Update()
        {
            base.Update();

            noteSheetBody.Width = DrawWidth;

            if (HoldStartTime != null)
            {
                var targetTime = HoldEndTime ?? Time.Current;
                var targetRatio = (float)Math.Clamp((targetTime - HitObject.StartTime) / HitObject.Duration, 0, 1);
                bodyContainer.Width = 1 - targetRatio;
            }
        }

        private class NoteSheetBody : CompositeDrawable
        {
            private const float border_size = 1f / 8f;

            private readonly Box backgroundBox;
            private readonly Triangles triangles;
            private readonly Box topBox;
            private readonly Box bottomBox;

            public NoteSheetBody()
            {
                AddRangeInternal(new Drawable[]
                {
                    backgroundBox = new Box { RelativeSizeAxes = Axes.Both },
                    triangles = new Triangles { RelativeSizeAxes = Axes.Both, Alpha = 0.8f },
                    topBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Height = border_size,
                        Colour = Color4.White,
                        Origin = Anchor.TopCentre,
                        Anchor = Anchor.TopCentre,
                    },
                    bottomBox = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Height = border_size,
                        Colour = Color4.White,
                        Origin = Anchor.BottomCentre,
                        Anchor = Anchor.BottomCentre,
                    }
                });
            }

            public void UpdateColours(Color4 colour)
            {
                backgroundBox.Colour = colour.Darken(1f);
                topBox.Colour = bottomBox.Colour = colour;
                triangles.Colour = colour;
                triangles.Alpha = 0.8f;
            }
        }
    }
}
