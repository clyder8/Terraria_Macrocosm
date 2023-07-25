using Macrocosm.Common.Utils;
using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using static Terraria.ModLoader.PlayerDrawLayer;

namespace Macrocosm.Content.Projectiles.Friendly.Melee
{
    public class ArtemiteGreatswordSwing : ModProjectile
	{
		public override string Texture => Macrocosm.TextureAssetsPath + "Swing";

		public override void SetDefaults()
		{
			Projectile.width = 120;
			Projectile.height = 120;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.tileCollide = false;
			Projectile.ignoreWater = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.ownerHitCheck = true;
			Projectile.ownerHitCheckDistance = 300f;
			Projectile.usesOwnerMeleeHitCD = true;
			Projectile.stopsDealingDamageAfterPenetrateHits = true;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
		}

		private Vector2 positionAdjustment => new Vector2(55 * Projectile.scale, 0).RotatedBy(Projectile.rotation);

		public override void AI()
		{
			float scaleFactor = 1.8f;
			float baseScale = 0f;
			float direction = Projectile.ai[0];
			float progress = Projectile.localAI[0] / Projectile.ai[1];
			Player player = Main.player[Projectile.owner];
			Item item = player.HeldItem;
			float speed = player.GetTotalAttackSpeed(DamageClass.Melee);

			Projectile.localAI[0] += 1.73f * speed;

			Projectile.rotation = (float)Math.PI * direction * progress + Projectile.velocity.ToRotation() + direction * (float)Math.PI + player.fullRotation;
			Projectile.Center = player.RotatedRelativePoint(player.MountedCenter) + positionAdjustment;

			Projectile.scale = baseScale + MathHelper.SmoothStep(0,1,progress) * scaleFactor;
			Projectile.scale *= player.GetAdjustedItemScale(item);
			Projectile.scale *= 1.5f;

			Vector2 hitboxPos = Projectile.Center - positionAdjustment + Utility.PolarVector(175, Projectile.rotation);

			for (int i = 0; i < (int)(4 * (Projectile.scale < 0.5f ? 0 : 1)); i++)
			{
				Vector2 dustVelocity = new Vector2(Main.rand.NextFloat(1, 10 * speed), 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 * Projectile.direction) + Main.player[Projectile.owner].velocity;
				Dust dust = Dust.NewDustDirect(hitboxPos, 1, 1, ModContent.DustType<ArtemiteBrightDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(2f, 3f));
				dust.noGravity = true;

				dustVelocity = new Vector2(Main.rand.NextFloat(4, 6), 0).RotatedBy(Projectile.rotation - MathHelper.PiOver2 * Projectile.direction) + Main.player[Projectile.owner].velocity;
				dust = Dust.NewDustDirect(Vector2.Lerp(Projectile.position, player.Center, 0.5f), Projectile.width / 2, Projectile.height / 2, ModContent.DustType<ArtemiteDust>(), dustVelocity.X, dustVelocity.Y, Scale: Main.rand.NextFloat(0.6f, 1f)); ;
				dust.noGravity = true;
			}

			if (Projectile.localAI[0] >= Projectile.ai[1] + 1)
 				Projectile.Kill();
 		}

		public override bool PreDraw(ref Color lightColor)
		{
			Texture2D texture = TextureAssets.Projectile[Type].Value;
			Texture2D star = ModContent.Request<Texture2D>(Macrocosm.TextureAssetsPath + "Star1").Value;

			Rectangle frame = texture.Frame(1, 4, frameY: 3);
			Vector2 origin = frame.Size() / 2f;

			Vector2 position = Projectile.Center - positionAdjustment - Main.screenPosition;
			SpriteEffects effects = Projectile.ai[0] < 0f ? SpriteEffects.FlipVertically : SpriteEffects.None;

			float progress = Projectile.localAI[0] / Projectile.ai[1];
			float progressScale = Utils.Remap(progress, 0f, 0.6f, 0f, 1f) * Utils.Remap(progress, 0.6f, 1f, 1f, 0f);

			Color color = new Color(130, 220, 199).NewAlpha(1f - progress);// * lightColor.GetLuminance();
	
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 0), color * progressScale, Projectile.rotation + Projectile.ai[0] * ((float)Math.PI / 4f) * -1f * (1f - progress), origin, Projectile.scale * 0.95f, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 1), color * 0.15f, Projectile.rotation, origin, Projectile.scale, effects, 0f);
			Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.7f * progressScale * 0.3f, Projectile.rotation, origin, Projectile.scale, effects, 0f);
		    Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 2), color * 0.8f * progressScale * 0.5f, Projectile.rotation, origin, Projectile.scale * 0.975f, effects, 0f);
		    Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (Color.White * (0.2f - 0.2f * progressScale)).NewAlpha(0.4f - 0.4f * progressScale), Projectile.rotation, origin, Projectile.scale * 0.95f, effects, 0f);
		    Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).NewAlpha(0.2f - 0.2f * progressScale), Projectile.rotation, origin, Projectile.scale * 0.75f, effects, 0f);
		    Main.EntitySpriteDraw(texture, position, texture.Frame(1, 4, frameY: 3), (color * progressScale).NewAlpha(0.1f - 0.05f *progressScale), Projectile.rotation, origin, Projectile.scale * 0.55f, effects, 0f);


			var state = Main.spriteBatch.SaveState();
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(BlendState.Additive, state);

			if (progress < 0.95f)
				for(int i = 0; i < 3; i++)
				{
					float scale = Projectile.scale * (0.02f - 0.003f * i);
					float angle = Projectile.rotation + (i * (MathHelper.PiOver4 / 2f));
					Main.EntitySpriteDraw(star, position + Utility.PolarVector(76 * Projectile.scale, angle), null, (new Color(168, 255, 255) * (1f + color.A/255f)).NewAlpha(1f), Projectile.rotation + MathHelper.PiOver4, star.Size() / 2f, scale, SpriteEffects.None);
				}

			Main.spriteBatch.End();
			Main.spriteBatch.Begin(state);
			return false;
		}
	}
}