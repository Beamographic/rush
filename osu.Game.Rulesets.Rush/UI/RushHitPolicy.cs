// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Game.Rulesets.Objects;
using osu.Game.Rulesets.Objects.Drawables;
using osu.Game.Rulesets.Rush.Objects;
using osu.Game.Rulesets.Rush.Objects.Drawables;

namespace osu.Game.Rulesets.Rush.UI
{
    public class RushHitPolicy
    {
        private readonly RushPlayfield playfield;

        private IEnumerable<DrawableHitObject> aliveHitObjects => playfield.AllAliveHitObjects;

        public RushHitPolicy(RushPlayfield playfield)
        {
            this.playfield = playfield;
        }

        public bool IsHittable(DrawableHitObject hitObject) => enumerateHitObjectsUpTo(hitObject.HitObject).All(obj => obj.AllJudged);

        private IEnumerable<DrawableHitObject> enumerateHitObjectsUpTo(HitObject hitobject)
        {
            double targetTime = hitobject.StartTime;

            foreach (var obj in aliveHitObjects)
            {
                if (obj.HitObject.StartTime >= targetTime)
                    yield break;

                var laned = hitobject as LanedHit;

                if (laned != null)
                {
                    // We don't want objects from another lane to block inputs
                    if (obj is IDrawableLanedHit lanedTarget)
                        if (laned.Lane != lanedTarget.Lane)
                            continue;
                }

                switch (obj)
                {
                    // We have to make sure DualHitParts from another lane don't block
                    case DrawableDualHit dual:
                        if (laned != null)
                            yield return laned.Lane == LanedHitLane.Air ? dual.Air : dual.Ground;
                        else
                            yield return dual;

                        break;

                    case DrawableStarSheet sheet:
                        yield return sheet.Head;

                        break;

                    default:
                        yield return obj;

                        break;
                }
            }
        }
    }
}
