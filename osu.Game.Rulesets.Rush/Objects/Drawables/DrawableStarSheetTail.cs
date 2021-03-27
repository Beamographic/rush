// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Rulesets.Scoring;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableStarSheetTail : DrawableStarSheetCap<StarSheetTail>
    {
        internal const double RELEASE_WINDOW_LENIENCE = 3;

        protected override RushSkinComponents Component => RushSkinComponents.StarSheetTail;

        public DrawableStarSheetTail()
            : this(null)
        {
        }

        public DrawableStarSheetTail(StarSheetTail starSheetTail)
            : base(starSheetTail)
        {
        }

        protected override void CheckForResult(bool userTriggered, double timeOffset)
        {
            var overallMissed = StarSheet.Result.Type == StarSheet.Result.Judgement.MinResult;

            // Apply tail miss at its time when the entire star sheet has already been judged as missed.
            if (overallMissed && timeOffset >= 0)
            {
                ApplyResult(r => r.Type = r.Judgement.MinResult);
                return;
            }

            // for now let's give the player an automatic perfect if they hold the action (like in a certain other rhythm game)
            if (!userTriggered)
            {
                if (timeOffset >= 0)
                    ApplyResult(r => r.Type = r.Judgement.MaxResult);
                return;
            }

            var result = HitObject.HitWindows.ResultFor(timeOffset);
            if (result == HitResult.None)
                return;

            // ...and an automatic perfect if they release within any "hit" judged period
            ApplyResult(r => r.Type = r.Judgement.MaxResult);
        }

        // FIXME: should logically be TrailingAnchor, not sure why it renders incorrectly
        protected override void AdjustAnchor()
        {
            if (HitObject is null) return;
            Anchor = LeadingAnchor;
        }
    }
}
