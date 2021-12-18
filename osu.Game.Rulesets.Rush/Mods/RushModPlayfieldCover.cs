// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
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

            List<Container> hitObjectAreas = new List<Container>{
                (Container)rushPlayfield.AirLane.HitObjectContainer.Parent,
                (Container)rushPlayfield.GroundLane.HitObjectContainer.Parent,
                (Container)rushPlayfield.HitObjectContainer.Parent,
            };


            foreach (var area in hitObjectAreas)
            {
                Container hitObjectAreaParent = (Container)area.Parent;
                hitObjectAreaParent.Remove(area);

                PlayfieldCoveringWrapper wrapper = new PlayfieldCoveringWrapper(area)
                {
                    RelativeSizeAxes = Axes.Both,
                    Direction = ExpandDirection,
                    Coverage = 0.5f,
                };

                hitObjectAreaParent.Add(wrapper);
            }
        }

        protected override void ApplyIncreasedVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }

        protected override void ApplyNormalVisibilityState(DrawableHitObject hitObject, ArmedState state)
        {
        }
    }
}
