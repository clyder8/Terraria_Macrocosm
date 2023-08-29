using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Projectiles.Friendly.Magic
{
	internal class ImbriumJewelProjectile : ModProjectile
	{
		public override void SetStaticDefaults()
		{
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

		public override void SetDefaults()
		{
			Projectile.width = 100;
			Projectile.height = 100;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.ignoreWater = true;
			Projectile.light = 1f;
            Projectile.penetrate = -1;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 24;
            height = 24;
            return false;
        }

        public override void AI()
        {
            bool collide = WorldGen.SolidTile(Projectile.Center.ToTileCoordinates());

			Projectile.alpha += 2;

			if (collide)
				Projectile.alpha += 8;
                    
			if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
                return;
            }

            Projectile.velocity *= MathHelper.SmoothStep(1f, 0.97f, Projectile.alpha / 255f);


            Projectile.ai[0]++;

            if (!collide && Projectile.alpha < 180 && Projectile.ai[0] >= 10)
            {
                Projectile.ai[0] -= 10;
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 shootVel = new Vector2(6, 6).RotatedByRandom(MathHelper.ToRadians(360));
                    Vector2 vector = new (Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
                    Projectile.NewProjectile(Projectile.InheritSource(Projectile), vector, shootVel, ModContent.ProjectileType<ImbriumJewelMeteor>(), (int)(Projectile.damage), Projectile.knockBack, Main.player[Projectile.owner].whoAmI);
                }
            }

            /*Projectile.ai[1]++;
            if (Projectile.ai[1] >= 15)
            {
                Projectile.ai[1] -= 15;
                if (Projectile.owner == Main.myPlayer)
                {
                    int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, Mod.Find<ModProjectile>("TheJewelOfShowersFx").Type, 0, 0, Main.player[Projectile.owner].whoAmI);
                    Main.projectile[proj].rotation = Projectile.rotation;
                }
            }*/

            Lighting.AddLight(Projectile.Center, 0f, 1f, 0f);
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X);

            if(Main.rand.NextFloat() > Projectile.alpha / 255f || Projectile.alpha < 120)
            {
				var star = Particle.CreateParticle<ImbriumStar>(new Vector2(Projectile.position.X, Projectile.position.Y) + Main.rand.NextVector2FromRectangle(new Rectangle(0, 0, (int)Projectile.Size.X, (int)Projectile.Size.Y)), Vector2.Zero, scale: 0.07f);
				star.Alpha = 1f - Projectile.alpha / 255f;
				//int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.GreenFairy, Projectile.velocity.X, Projectile.velocity.Y, 100, default(Color), 1.7f);
				//Main.dust[dust].noGravity = true;
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
			//Redraw the projectile with the color not influenced by light
			//Texture2D texture = (Texture2D)ModContent.Request<Texture2D>("Macrocosm/Content/Projectiles/Friendly/Magic/TheJewelOfShowersProj");

			Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Type].Value.Width * 0.5f, Projectile.height * 0.5f);

            var state = Main.spriteBatch.SaveState();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(BlendState.Additive, state);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
            }

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);

			return true;
        }
    }
}