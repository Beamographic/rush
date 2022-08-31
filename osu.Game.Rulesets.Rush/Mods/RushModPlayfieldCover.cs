// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Rulesets.Mods;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.UI;
using osu.Game.Rulesets.UI;

namespace osu.Game.Rulesets.Rush.Mods
{
    public abstract class RushModPlayfieldCover : ModHidden, IApplicableToDrawableRuleset<RushHitObject>
    {
        public override Type[] IncompatibleMods => new[] { typeof(ModFlashlight<RushHitObject>) };

        /// <summary>
        /// The direction in which the cover should expand.
        /// </summary>
        protected abstract CoverExpandDirection ExpandDirection { get; }

        public virtual void ApplyToDrawableRuleset(DrawableRuleset<RushHitObject> drawableRuleset)
        {
            RushPlayfield rushPlayfield = (RushPlayfield)drawableRuleset.Playfield;

            Container hitObjectArea = rushPlayfield.HitObjectArea;
            Container hocParent = (Container)hitObjectArea.Parent;
            hocParent.Remove(hitObjectArea);

            PlayfieldCoveringWrapper wrapper = new PlayfieldCoveringWrapper(hitObjectArea)
            {
                RelativeSizeAxes = Axes.Both,
                Direction = ExpandDirection,
                Coverage = 0.5f,
            };

            hocParent.Add(wrapper);
        }

        protected override void ApplyIncreasedVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }

        protected override void ApplyNormalVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }
    }
}
