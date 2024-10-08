﻿using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;

namespace Macrocosm.Content.Particles
{
    public class RocketExhaustSmoke : Particle
    {
        public override int FrameCount => 3;
        public override bool SetRandomFrameOnSpawn => true;

        public bool FadeIn = false;
        public bool FadeOut = true;

        public int TargetAlpha = 255;

        public int FadeInSpeed = 1;
        public int FadeOutSpeed = 4;

        public float ScaleDownSpeed = 0.005f;

        public bool Collide;

        private float alpha = 255;
        private bool fadedIn = false;
        private bool collided = false;

        public override ParticleDrawLayer DrawLayer => collided ? ParticleDrawLayer.AfterProjectiles : ParticleDrawLayer.BeforeNPCs;

        public override void SetDefaults()
        {
            TimeToLive = 120;

            Acceleration = new(0, -0.98f);
            ScaleVelocity = new(-0.005f);

            FadeIn = false;
            FadeOut = true;

            TargetAlpha = 255;

            FadeInSpeed = 1;
            FadeOutSpeed = 4;

            ScaleDownSpeed = 0.005f;

            Collide = false;

            alpha = 255;
            fadedIn = false;
            collided = false;
        }

        public override void OnSpawn()
        {
            if (FadeOut)
                alpha = 255;

            if (FadeIn)
                alpha = 0;
        }

        public override void AI()
        {
            if (FadeIn && FadeOut)
            {
                if (!fadedIn)
                {
                    if (alpha < TargetAlpha)
                        alpha += FadeInSpeed;
                    else
                        fadedIn = true;
                }
                else if (alpha > 0)
                {
                    alpha -= FadeOutSpeed;
                }
            }
            else
            {
                if (FadeIn && alpha < TargetAlpha)
                    alpha += FadeInSpeed;

                if (FadeOut && alpha > 0)
                    alpha -= FadeOutSpeed;
            }

            if (Collide)
            {
                var originalVelocity = Velocity;

                var collisionVelocity = Collision.TileCollision(Position, Velocity, 1, 1);

                if (originalVelocity != collisionVelocity && !collided)
                {
                    collided = true;
                    Velocity = collisionVelocity;
                    Velocity.Y = -MathF.Abs(Velocity.Y);
                    Velocity.Y *= Main.rand.NextFloat(0.1f, 0.25f);
                    Velocity.X *= Main.rand.NextFloat(5f, 10f);
                }

                if (collided)
                {
                    Velocity.Y *= 1.01f;
                }
            }

            alpha = (int)MathHelper.Clamp(alpha, 0, 255);

            if (Scale.X < 0.1f || (fadedIn && alpha <= 0))
                Kill();
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 screenPosition, Color lightColor)
        {
            spriteBatch.Draw(Texture.Value, Position - screenPosition, GetFrame(), Color.Lerp(Color, lightColor, 0.5f).WithAlpha(Color.A) * ((float)alpha / 255f), Rotation, Size * 0.5f, Scale, SpriteEffects.None, 0f);
        }

    }
}
