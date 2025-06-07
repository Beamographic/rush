// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;
using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Rush.Objects;
using osuTK;

namespace osu.Game.Rulesets.Rush.Beatmaps
{
    public class RushBeatmap : Beatmap<RushHitObject>
    {
        public override IEnumerable<BeatmapStatistic> GetStatistics()
        {
            int minions = HitObjects.Count(s => s is Minion);
            int starsheets = HitObjects.Count(s => s is StarSheet);
            int sawblades = HitObjects.Count(s => s is Sawblade);
            int dualhits = HitObjects.Count(s => s is DualHit);
            int minibosses = HitObjects.Count(s => s is MiniBoss);
            int hearts = HitObjects.Count(s => s is Heart);

            float total = Math.Max(minions + starsheets + sawblades + dualhits + minibosses + hearts, 1);

            return new[]
            {
                new BeatmapStatistic
                {
                    Name = @"Minions",
                    Content = minions.ToString(),
                    CreateIcon = () => createIcon(FontAwesome.Regular.Angry),
                    BarDisplayLength = minions / total
                },
                new BeatmapStatistic
                {
                    Name = @"Star Sheets",
                    Content = starsheets.ToString(),
                    CreateIcon = () => createIcon(FontAwesome.Regular.Star),
                    BarDisplayLength = starsheets / total
                },
                new BeatmapStatistic
                {
                    Name = @"Dual Hits",
                    Content = dualhits.ToString(),
                    CreateIcon = () => createIcon(FontAwesome.Solid.Cog),
                    BarDisplayLength = dualhits / total
                },
                new BeatmapStatistic
                {
                    Name = @"Sawblades",
                    Content = sawblades.ToString(),
                    CreateIcon = () => createIcon(FontAwesome.Solid.Sun),
                    BarDisplayLength = sawblades / total
                },
                new BeatmapStatistic
                {
                    Name = @"Minibosses",
                    Content = minibosses.ToString(),
                    CreateIcon = () => createIcon(FontAwesome.Solid.Mitten),
                    BarDisplayLength = minibosses / total
                },
                new BeatmapStatistic
                {
                    Name = @"Hearts",
                    Content = hearts.ToString(),
                    CreateIcon = () => createIcon(FontAwesome.Solid.Heart),
                    BarDisplayLength = hearts / total
                }
            };
        }

        private static Drawable createIcon(IconUsage icon) => new SpriteIcon { Icon = icon, Scale = new Vector2(0.7f) };
    }
}
