// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Input;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableStarSheet : DrawableLanedHit<StarSheet>
    {
        public const float NOTE_SHEET_SIZE = RushPlayfield.HIT_TARGET_SIZE * 0.75f;
        public const float REQUIRED_COMPLETION = 0.75f;

        public DrawableStarSheetHead Head => headContainer.Child;
        public DrawableStarSheetTail Tail => tailContainer.Child;

        private readonly Container<DrawableStarSheetHead> headContainer;
        private readonly Container<DrawableStarSheetTail> tailContainer;

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

        public override bool DisplayResult => false;

        public DrawableStarSheet()
            : this(null)
        {
        }

        public DrawableStarSheet(StarSheet hitObject)
            : base(hitObject)
        {
            Height = NOTE_SHEET_SIZE;

            AddRangeInternal(new[]
            {
                bodyContainer = new Container<SkinnableDrawable>
                {
                    Masking = true,
                    RelativeSizeAxes = Axes.Both,
                    Child = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.StarSheetBody), _ => new StarSheetBodyPiece())
                },
                headContainer = new Container<DrawableStarSheetHead> { RelativeSizeAxes = Axes.Both },
                tailContainer = new Container<DrawableStarSheetTail> { RelativeSizeAxes = Axes.Both },
                holdCap = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.StarSheetHold), _ => new StarSheetCapStarPiece())
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
                case DrawableStarSheetHead head:
                    headContainer.Child = head;
                    break;

                case DrawableStarSheetTail tail:
                    tailContainer.Child = tail;
                    break;
            }
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            headContainer.Clear(false);
            tailContainer.Clear(false);
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case StarSheetHead head:
                    return new DrawableStarSheetHead(head)
                    {
                        Anchor = LeadingAnchor,
                        Origin = Anchor.Centre,
                        AccentColour = { BindTarget = AccentColour }
                    };

                case StarSheetTail tail:
                    return new DrawableStarSheetTail(tail)
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
                if (Head.Result.Type == Head.Result.Judgement.MinResult)
                {
                    ApplyResult(r => r.Type = r.Judgement.MinResult);
                    return;
                }

                if (userTriggered)
                    Head.TriggerResult();

                return;
            }

            // Released before required progress for completion, judge as overall missed.
            if (userTriggered && Progress < REQUIRED_COMPLETION)
            {
                ApplyResult(r => r.Type = r.Judgement.MinResult);
                return;
            }

            // Judge the tail if triggered by user.
            if (userTriggered)
                Tail.TriggerResult();

            // Wait until the tail is judged.
            if (!Tail.Judged)
                return;

            ApplyResult(r => r.Type = r.Judgement.MaxResult);
        }

        public override bool OnPressed(RushAction action)
        {
            if (!action.IsLaneAction())
                return false;

            if (!LaneMatchesAction(action) || Head.Judged)
                return false;

            if (!CheckHittable(this))
                return false;

            UpdateResult(true);
            return Head.Judged;
        }

        public override void OnReleased(RushAction action)
        {
            // Note sheet not held yet (i.e. not our time yet) or already broken / finished.
            if (Judged || !Head.IsHit)
                return;

            if (!LaneMatchesAction(action))
                return;

            // Check if there was also another action holding the same star sheet,
            // and use it in replace to this released one if so. (support for switching keys during hold)
            if (ActionInputManager.PressedActions.Count(LaneMatchesAction) > 1)
                return;

            UpdateResult(true);
        }

        protected override void Update()
        {
            base.Update();

            if (Head.IsHit && !Tail.IsHit)
            {
                holdCap.X = DrawWidth * (float)Progress;
                holdCap.Y = DrawHeight / 2f;
                holdCap.Show();
            }
            else
                holdCap.Hide();

            if (!IsHit)
            {
                // Keep the body piece width in-line with ours and
                // start cutting its container's width as we hold it.
                BodyDrawable.Width = DrawWidth;
                bodyContainer.Width = 1 - (float)Progress;
                bodyContainer.Show();
            }
            else
                bodyContainer.Hide();
        }

        public override void PlaySamples()
        {
            // nested hitobjects will play samples
        }
    }
}
