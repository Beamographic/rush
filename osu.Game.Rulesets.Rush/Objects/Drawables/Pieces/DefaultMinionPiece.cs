// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public class DefaultMinionPiece : CompositeDrawable
    {
        private readonly TextureAnimation animation;

        public DefaultMinionPiece()
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

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            animation.AddFrames(new[] { store.Get($"Minion/pippidon_air_0"), store.Get($"Minion/pippidon_air_1") });
        }
    }
}
