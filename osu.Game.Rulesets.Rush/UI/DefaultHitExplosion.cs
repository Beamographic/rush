// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;
using osuTK.Graphics;

namespace osu.Game.Rulesets.Rush.UI
{
    public class DefaultHitExplosion : CompositeDrawable
    {
        private readonly Sprite colouredExplosion;
        private readonly Sprite whiteExplosion;

        public override bool RemoveWhenNotAlive => true;

        public DefaultHitExplosion(Color4 explosionColour)
        {
            Origin = Anchor.Centre;

            InternalChildren = new Drawable[]
            {
                // TODO: flashbang
                colouredExplosion = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = explosionColour,
                    Scale = new Vector2(1f)
                },
                whiteExplosion = new Sprite
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Scale = new Vector2(0.75f)
                },
                // TODO: small particles
                // TODO: stars
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            colouredExplosion.Texture = store.Get("exp");
            whiteExplosion.Texture = store.Get("exp");
        }
    }
}
