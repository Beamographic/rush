// Copyright (c) Shane Woolcock. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Animations;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Textures;
using osu.Game.Rulesets.Objects.Drawables;

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

        [Resolved]
        private TextureStore textures { get; set; }

        [BackgroundDependencyLoader(true)]
        private void load([CanBeNull] DrawableHitObject drawableHitObject)
        {
            if (drawableHitObject is DrawableMinion drawableMinion)
                drawableMinion.OnHitObjectApplied += ApplyVisuals;
        }

        public void ApplyVisuals(LanedHit HitObject)
        {
            animation.ClearFrames();

            var laneStr = "air";
            laneStr = HitObject.Lane == LanedHitLane.Air ? "air" : "ground";

            animation.AddFrames(new[] { textures.Get($"Minion/pippidon_{laneStr}_0"), textures.Get($"Minion/pippidon_{laneStr}_1") });
        }
    }
}
