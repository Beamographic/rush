// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableNoteSheet : DrawableLanedHit<NoteSheet>
    {
        public const float NOTE_SHEET_SIZE = RushPlayfield.HIT_TARGET_SIZE * 0.75f;
        public const float REQUIRED_COMPLETION = 0.75f;

        public DrawableNoteSheetHead Head => headContainer.Child;
        public DrawableNoteSheetTail Tail => tailContainer.Child;

        private readonly Container<DrawableNoteSheetHead> headContainer;
        private readonly Container<DrawableNoteSheetTail> tailContainer;

        public Drawable BodyDrawable => bodyContainer.Child.Drawable;

        private readonly Container<SkinnableDrawable> bodyContainer;

        private readonly Drawable holdCap;

        private double? holdStartTime => !Head.IsHit ? (double?)null : HitObject.StartTime;
        private double? holdEndTime => !Judged ? (double?)null : (HitObject.EndTime + Result.TimeOffset);

        public double Progress
        {
            get
            {
                if (IsHit)
                    return 1.0;

                return Math.Clamp(((holdEndTime ?? Time.Current) - (holdStartTime ?? Time.Current)) / HitObject.Duration, 0.0, 1.0);
            }
        }

        [Resolved]
        private IScrollingInfo scrollingInfo { get; set; }

        [Resolved]
        private RushPlayfield playfield { get; set; }

        public override bool DisplayResult => false;

        public DrawableNoteSheet(NoteSheet hitObject)
            : base(hitObject)
        {
            Height = NOTE_SHEET_SIZE;

            Content.AddRange(new[]
            {
                bodyContainer = new Container<SkinnableDrawable>
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.Both,
                    Child = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.NoteSheetBody), _ => new NoteSheetBodyPiece())
                },
                headContainer = new Container<DrawableNoteSheetHead> { RelativeSizeAxes = Axes.Both },
                tailContainer = new Container<DrawableNoteSheetTail> { RelativeSizeAxes = Axes.Both },
                holdCap = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.NoteSheetHold), _ => new NoteSheetCapStarPiece())
                {
                    Origin = Anchor.Centre,
                }
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
                        Anchor = LeadingAnchor,
                        Origin = Anchor.Centre,
                        AccentColour = { BindTarget = AccentColour }
                    };

                case NoteSheetTail _:
                    return new DrawableNoteSheetTail(this)
                    {
                        Anchor = TrailingAnchor,
                        Origin = Anchor.Centre,
                        AccentColour = { BindTarget = AccentColour }
                    };
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            Origin = LeadingAnchor;
            headContainer.Origin = headContainer.Anchor = LeadingAnchor;
            tailContainer.Origin = tailContainer.Anchor = TrailingAnchor;

            BodyDrawable.Origin = BodyDrawable.Anchor = TrailingAnchor;
            bodyContainer.Origin = bodyContainer.Anchor = TrailingAnchor;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            // Check if the head hasn't been hit. if so then triggered by a press, judge the head.
            if (!Head.IsHit)
            {
                // Head missed, judge as overall missed.
                if (Head.Result.Type == HitResult.Miss)
                {
                    ApplyResult(r => r.Type = HitResult.Miss);
                    return;
                }

                if (userTriggered)
                    Head.TriggerResult();

                return;
            }

            // Released before required progress for completion, judge as overall missed.
            if (userTriggered && Progress < REQUIRED_COMPLETION)
            {
                ApplyResult(r => r.Type = HitResult.Miss);
                return;
            }

            // Judge the tail if triggered by user.
            if (userTriggered)
                Tail.TriggerResult();

            // Wait until the tail is judged.
            if (!Tail.Judged)
                return;

            // Determine the overall judgement for the object when the tail is judged.
            var minimumResult = (HitResult)Math.Min((int)Head.Result.Type, (int)Tail.Result.Type);
            ApplyResult(r => r.Type = minimumResult);
        }

        public override bool OnPressed(RushAction action)
        {
            if (!LaneMatchesAction(action) || Head.Judged)
                return false;

            UpdateResult(true);
            return Head.Judged;
        }

        public override void OnReleased(RushAction action)
        {
            if (!LaneMatchesAction(action))
                return;

            // Check if there was also another action holding the same note sheet,
            // and use it in replace to this released one if so. (support for switching keys during hold)
            if (ActionInputManager.PressedActions.Count(LaneMatchesAction) > 1)
                return;

            // Note sheet not held yet (i.e. not our time yet) or already broken / finished.
            if (!Head.IsHit || Judged)
                return;

            UpdateResult(true);
        }

        protected override void Update()
        {
            base.Update();

            if (Head.IsHit)
                holdCap.Show();
            else
                holdCap.Hide();

            holdCap.X = DrawWidth * (float)Progress;
            holdCap.Y = DrawHeight / 2f;

            // Keep the body piece width in-line with ours and
            // start cutting its container's width as we hold it.
            BodyDrawable.Width = DrawWidth;
            bodyContainer.Width = 1 - (float)Progress;
        }

        public override void PlaySamples()
        {
            // nested hitobjects will play samples
        }
    }
}
