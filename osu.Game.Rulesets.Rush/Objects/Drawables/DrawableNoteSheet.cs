// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableNoteSheet : DrawableLanedHit<NoteSheet>
    {
        public const float NOTE_SHEET_SIZE = RushPlayfield.HIT_TARGET_SIZE * 0.75f;
        public const float REQUIRED_COMPLETION = 0.75f;

        public Bindable<bool> HasBroken { get; } = new BindableBool();

        public DrawableNoteSheetHead Head => headContainer.Child;
        public DrawableNoteSheetTail Tail => tailContainer.Child;
        public DrawableNoteSheetBody Body => bodyContainer.Child;

        private readonly Container<DrawableNoteSheetBody> bodyContainer;
        private readonly Container<DrawableNoteSheetHead> headContainer;
        private readonly Container<DrawableNoteSheetTail> tailContainer;

        private readonly DrawableNoteSheetCapStar holdStar;

        private int pressCount;

        public double? HoldStartTime { get; private set; }
        public double? HoldEndTime { get; private set; }
        public double CompletionAmount => ((HoldEndTime ?? HoldStartTime ?? 0) - (HoldStartTime ?? 0)) / HitObject.Duration;

        [Resolved]
        private IScrollingInfo scrollingInfo { get; set; }

        [Resolved]
        private RushPlayfield playfield { get; set; }

        public DrawableNoteSheet(NoteSheet hitObject)
            : base(hitObject)
        {
            Height = NOTE_SHEET_SIZE;

            Content.AddRange(new Drawable[]
            {
                bodyContainer = new Container<DrawableNoteSheetBody>
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                },
                headContainer = new Container<DrawableNoteSheetHead> { RelativeSizeAxes = Axes.Both },
                tailContainer = new Container<DrawableNoteSheetTail> { RelativeSizeAxes = Axes.Both },
                holdStar = new DrawableNoteSheetCapStar
                {
                    Origin = Anchor.Centre,
                    Size = new Vector2(NOTE_SHEET_SIZE),
                    Alpha = 0f,
                }
            });

            AccentColour.ValueChanged += _ => updateHoldStar();
            HasBroken.ValueChanged += _ => updateHoldStar();
        }

        private void updateHoldStar() => holdStar.UpdateColour(HasBroken.Value ? Color4.Gray : AccentColour.Value);

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

                case DrawableNoteSheetBody body:
                    bodyContainer.Child = body;
                    break;
            }
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            headContainer.Clear();
            tailContainer.Clear();
            bodyContainer.Clear();
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case NoteSheetHead _:
                    return new DrawableNoteSheetHead(this)
                    {
                        Anchor = LeadingAnchor,
                        Origin = Anchor.Centre,
                        AccentColour = { BindTarget = AccentColour },
                        HasBroken = { BindTarget = HasBroken },
                    };

                case NoteSheetTail _:
                    return new DrawableNoteSheetTail(this)
                    {
                        Anchor = TrailingAnchor,
                        Origin = Anchor.Centre,
                        AccentColour = { BindTarget = AccentColour },
                        HasBroken = { BindTarget = HasBroken },
                    };

                case NoteSheetBody _:
                    return new DrawableNoteSheetBody(this)
                    {
                        Anchor = TrailingAnchor,
                        Origin = TrailingAnchor,
                        AccentColour = { BindTarget = AccentColour },
                        HasBroken = { BindTarget = HasBroken },
                    };
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            Origin = LeadingAnchor;
            bodyContainer.Origin = bodyContainer.Anchor = TrailingAnchor;
            tailContainer.Origin = tailContainer.Anchor = TrailingAnchor;
            headContainer.Origin = headContainer.Anchor = LeadingAnchor;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged || timeOffset < 0)
                return;

            Tail.UpdateResult();

            if (Tail.IsHit && Head.IsHit && !HasBroken.Value)
                ApplyResult(r => r.Type = HitResult.Perfect);
            else
                ApplyResult(r => r.Type = HitResult.Miss);
        }

        public override bool OnPressed(RushAction action)
        {
            if (AllJudged || HasBroken.Value)
                return false;

            if (!LaneMatchesAction(action))
                return false;

            pressCount++;

            if (pressCount > 1)
                return true;

            beginHoldAt(Time.Current - Head.HitObject.StartTime);
            Head.UpdateResult();

            return true;
        }

        private void beginHoldAt(double timeOffset)
        {
            if (timeOffset < -Head.HitObject.HitWindows.WindowFor(HitResult.Miss))
                return;

            HoldStartTime = Time.Current;
        }

        public override void OnReleased(RushAction action)
        {
            if (!LaneMatchesAction(action))
                return;

            pressCount--;

            if (pressCount > 0 || AllJudged)
                return;

            if (HasBroken.Value || HoldStartTime == null || HoldEndTime != null)
                return;

            HoldEndTime = Time.Current;

            var tailOffset = Time.Current - Tail.HitObject.StartTime;
            tailOffset /= DrawableNoteSheetTail.RELEASE_WINDOW_LENIENCE;

            if (CompletionAmount < REQUIRED_COMPLETION)
                HasBroken.Value = true;
            else if (Tail.HitObject.HitWindows.CanBeHit(tailOffset))
            {
                Tail.UpdateResult();
                HasBroken.Value = !Tail.IsHit;
            }
        }

        protected override void Update()
        {
            base.Update();

            if (Head.IsHit || HasBroken.Value)
                holdStar.Show();
            else
                holdStar.Hide();

            float targetRatio = 0;

            if (HoldStartTime != null)
            {
                var targetTime = HoldEndTime ?? Time.Current;
                targetRatio = (float)Math.Clamp((targetTime - HitObject.StartTime) / HitObject.Duration, 0, 1);
            }

            bodyContainer.Width = 1 - targetRatio;
            holdStar.Y = DrawHeight / 2f;
            holdStar.X = DrawWidth * targetRatio;
        }
    }
}
