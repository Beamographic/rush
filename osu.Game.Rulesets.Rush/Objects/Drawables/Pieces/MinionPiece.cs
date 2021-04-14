// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public class MinionPiece : CompositeDrawable
    {
        private readonly TextureAnimation animation;

        public MinionPiece()
        {
            RelativeSizeAxes = Axes.Both;

            InternalChild = animation = new TextureAnimation
            {
                Origin = Anchor.Centre,
                Anchor = Anchor.Centre,
                FillMode = FillMode.Fit,
                RelativeSizeAxes = Axes.Both,
                DefaultFrameLength = 250,
            };
        }

        [BackgroundDependencyLoader(true)]
        private void load(IDrawableLanedHit laned, TextureStore textures)
        {
            var laneStr = laned.Lane == LanedHitLane.Air ? "air" : "ground";

            animation.AddFrames(new[] { textures.Get($"Minion/pippidon_{laneStr}_0"), textures.Get($"Minion/pippidon_{laneStr}_1") });
        }
    }
}
