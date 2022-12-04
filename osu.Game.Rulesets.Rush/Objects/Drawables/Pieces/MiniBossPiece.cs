// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;

namespace osu.Game.Rulesets.Rush.Objects.Drawables.Pieces
{
    public partial class MiniBossPiece : CompositeDrawable
    {
        private readonly TextureAnimation normalAnimation;
        private readonly TextureAnimation hitAnimation;

        public MiniBossPiece()
        {
            Origin = Anchor.Centre;
            Anchor = Anchor.Centre;

            AddRangeInternal(new Drawable[]
            {
                normalAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    DefaultFrameLength = 250,
                },
                hitAnimation = new TextureAnimation
                {
                    Origin = Anchor.Centre,
                    Anchor = Anchor.Centre,
                    DefaultFrameLength = 250,
                    Alpha = 0f
                },
            });
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            normalAnimation.AddFrames(new[] { store.Get("MiniBoss/pippidon_boss_0"), store.Get("MiniBoss/pippidon_boss_1") });
            hitAnimation.AddFrames(new[] { store.Get("MiniBoss/pippidon_boss_hurt_0"), store.Get("MiniBoss/pippidon_boss_hurt_1"), store.Get("MiniBoss/pippidon_boss_hurt_2") });
        }
    }
}
