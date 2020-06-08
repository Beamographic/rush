// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Rush.Objects;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public class RushBeatmap : Beatmap<RushHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            int minions = HitObjects.Count(s => s is Minion);
            int notesheets = HitObjects.Count(s => s is NoteSheet);
            int sawblades = HitObjects.Count(s => s is Sawblade);
            int dualorbs = HitObjects.Count(s => s is DualOrb);
            int minibosses = HitObjects.Count(s => s is MiniBoss);
            int hearts = HitObjects.Count(s => s is Heart);

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = @"Minion Count",
                    Content = minions.ToString(),
                    Icon = FontAwesome.Regular.Angry
                },
                new BeatmapStatistic
                {
                    Name = @"Notesheet Count",
                    Content = notesheets.ToString(),
                    Icon = FontAwesome.Regular.Star
                },
                new BeatmapStatistic
                {
                    Name = @"Dual Orb Count",
                    Content = dualorbs.ToString(),
                    Icon = FontAwesome.Solid.Cog
                },
                new BeatmapStatistic
                {
                    Name = @"Sawblade Count",
                    Content = sawblades.ToString(),
                    Icon = FontAwesome.Solid.Sun
                },
                new BeatmapStatistic
                {
                    Name = @"Miniboss Count",
                    Content = minibosses.ToString(),
                    Icon = FontAwesome.Solid.Mitten
                },
                new BeatmapStatistic
                {
                    Name = @"Heart Count",
                    Content = hearts.ToString(),
                    Icon = FontAwesome.Solid.Heart
                }
            };
        }
    }
}
