// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects.Drawables.Pieces;
using osu.Game.Rulesets.Scoring;
using osu.Game.Rulesets.UI.Scrolling;
using osu.Game.Skinning;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableDualHit : DrawableRushHitObject<DualHit>
    {
        private Container<DrawableDualHitPart> partContainer;
        private SkinnableDrawable skinnedJoin;

        public IEnumerable<(LanedHitLane, bool)> LanesHit => partContainer.Select(part => (part.Lane, part.IsHit));

        public DrawableDualHit()
            : this(null)
        {
        }

        public DrawableDualHit([CanBeNull] DualHit hitObject = null)
            : base(hitObject)
        {
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            RelativeSizeAxes = Axes.Y;
            Height = 1f;

            Content.AddRange(new Drawable[]
            {
                skinnedJoin = new SkinnableDrawable(new RushSkinComponent(RushSkinComponents.DualHitJoin), _ => new DualHitJoinPiece()),
                partContainer = new Container<DrawableDualHitPart> { RelativeSizeAxes = Axes.Both },
            });
        }

        protected override void AddNestedHitObject(DrawableHitObject hitObject)
        {
            base.AddNestedHitObject(hitObject);

            switch (hitObject)
            {
                case DrawableDualHitPart part:
                    partContainer.Add(part);
                    break;
            }
        }

        protected override void ClearNestedHitObjects()
        {
            base.ClearNestedHitObjects();
            partContainer.Clear(false);
        }

        protected override DrawableHitObject CreateNestedHitObject(HitObject hitObject)
        {
            switch (hitObject)
            {
                case DualHitPart part:
                    return new DrawableDualHitPart(part);
            }

            return base.CreateNestedHitObject(hitObject);
        }

        protected override void UpdateInitialTransforms()
        {
            base.UpdateInitialTransforms();

            skinnedJoin.Show();
        }

        protected override void OnDirectionChanged(ValueChangedEvent<ScrollingDirection> e)
        {
            base.OnDirectionChanged(e);

            Origin = e.NewValue == ScrollingDirection.Left ? Anchor.CentreLeft : Anchor.CentreRight;
        }

        public override bool OnPressed(RushAction action)
        {
            if (AllJudged)
                return false;

            bool anyResults = false;
            bool allResults = true;

            foreach (DrawableDualHitPart part in partContainer)
            {
                bool result = !part.AllJudged && part.LaneMatchesAction(action) && part.UpdateResult(true);
                anyResults |= result;
                allResults &= result;
            }

            return allResults ? UpdateResult(true) : anyResults;
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            if (AllJudged)
                return;

            if (!userTriggered && timeOffset < 0)
                return;

            foreach (DrawableDualHitPart part in partContainer)
            {
                if (!part.AllJudged)
                    part.UpdateResult(false);
            }

            if (partContainer.Any(part => part.Result.Type == HitResult.None))
                return;

            // If we missed either, it's an overall miss.
            // If we hit both, the overall judgement is the lowest score of the two.
            ApplyResult(r =>
            {
                var lowestResult = partContainer.Min(part => part.Result.Type);
                r.Type = partContainer.All(part => !part.IsHit) ? HitResult.Miss : lowestResult;
            });
        }

        protected override void UpdateStateTransforms(ArmedState state)
        {
            const float animation_time = 300f;

            switch (state)
            {
                case ArmedState.Idle:
                    UnproxyContent();
                    break;

                case ArmedState.Miss:
                    this.FadeOut(animation_time);
                    break;

                case ArmedState.Hit:
                    ProxyContent();
                    skinnedJoin.Hide();
                    this.FadeOut(animation_time);
                    break;
            }
        }
    }
}
