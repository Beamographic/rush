// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;

namespace osu.Game.Rulesets.Dash.UI
{
    public class DashPlayerSprite : Sprite, IKeyBindingHandler<DashAction>
    {
        private const float punch_time = 300f;
        private const float travel_time = 150f;

        private Texture standingTexture;
        private Texture[] punchAir;
        private Texture[] punchGround;

        private int punchIndex = 0;
        private int texCount = 0;

        private readonly float groundY;
        private readonly float airY;

        public DashPlayerSprite(float groundY, float airY)
        {
            this.groundY = groundY;
            this.airY = airY;
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            standingTexture = store.Get("reimu_standing");
            punchAir = new[] { store.Get("reimu_air_1"), store.Get("reimu_air_2") };
            punchGround = new[] { store.Get("reimu_ground_1"), store.Get("reimu_ground_2") };
            texCount = Math.Min(punchAir.Length, punchGround.Length);

            Texture = standingTexture;
        }

        public bool OnPressed(DashAction action)
        {
            ClearTransforms();

            switch (action)
            {
                default:
                case DashAction.AirPrimary:
                case DashAction.AirSecondary:
                    Texture = punchAir[punchIndex];
                    this.MoveToY(airY, travel_time, Easing.Out)
                        .Then().Delay(punch_time)
                        .Then().MoveToY(groundY, travel_time, Easing.In)
                        .OnComplete(_ => Texture = standingTexture);
                    break;

                case DashAction.GroundPrimary:
                case DashAction.GroundSecondary:
                    Texture = punchGround[punchIndex];
                    this.MoveToY(groundY, travel_time, Easing.In)
                        .Then().Delay(punch_time)
                        .Then().MoveToY(groundY)
                        .OnComplete(_ => Texture = standingTexture);
                    break;
            }

            punchIndex = (punchIndex + 1) % texCount;

            return false;
        }

        public void OnReleased(DashAction action)
        {
        }
    }
}
