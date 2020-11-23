// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Pooling;
using osu.Game.Rulesets.Objects.Drawables;

namespace osu.Game.Rulesets.Rush.Objects.Drawables
{
    public class DrawableRushPool<T> : DrawablePool<T>
        where T : DrawableHitObject, new()
    {
        private readonly Action<Drawable> onLoaded;

        public DrawableRushPool(Action<Drawable> onLoaded, int initialSize, int? maximumSize = null)
            : base(initialSize, maximumSize)
        {
            this.onLoaded = onLoaded;
        }

        protected override T CreateNewDrawable() => base.CreateNewDrawable().With(o =>
        {
            if (o is DrawableRushHitObject rushHitObject)
                rushHitObject.OnLoadComplete += onLoaded;
        });
    }
}
