using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Macrocosm.Common;
using Macrocosm.Common.Utils;
using Macrocosm.Common.Netcode;
using Macrocosm.Content.Biomes;
using Macrocosm.Common.Drawing.Particles;
using Macrocosm.Content.Particles;
using System.IO;

namespace Macrocosm.Content.NPCs.Enemies.Moon
{
	public class ZombieSecurity : MoonEnemy
	{
		public enum ActionState 
		{ 
			Walk,
			Shoot 
		}

		[NetSync] public ActionState AI_State = ActionState.Walk;

		public enum AimType { Horizontal, Upwards, Downwards }
		[NetSync] public AimType AimMode;

		[NetSync] public int ShootCooldownCounter = maxCooldown;
		[NetSync] public int ShootSequenceCounter = maxShootSequence;

		public Player TargetPlayer => Main.player[NPC.target];

		#region Privates 
		private readonly IntRange shootFramesCommon = new(0, 5);
		private readonly IntRange shootFramesHorizontal = new(6, 10);
		private readonly IntRange shootFramesUpwards = new(11, 15);
		private readonly IntRange shootFramesDownward = new(16, 20);
		private readonly IntRange walkFrames = new(21, 30);
		private const int fallingFrame = 31;

		private const int maxCooldown = 320;
		private const int maxShootSequence = 84;
		private const int sequenceShoot = 72;
		private const int visualShoot = 64;
		#endregion

		public override void SetStaticDefaults()
		{
			Main.npcFrameCount[Type] = 32;
		}

		public override void SetDefaults()
		{
			NPC.width = 24;
			NPC.height = 44;
			NPC.damage = 60;
			NPC.defense = 60;
			NPC.lifeMax = 2200;
			NPC.HitSound = SoundID.NPCHit1;
			NPC.DeathSound = SoundID.NPCDeath2;
			NPC.knockBackResist = 0.5f;
			NPC.aiStyle = -1;
 			Banner = Item.NPCtoBanner(NPCID.Zombie);
			BannerItem = Item.BannerToItem(Banner);
			SpawnModBiomes = new int[1] { ModContent.GetInstance<MoonBiome>().Type }; // Associates this NPC with the Moon Biome in Bestiary
		}

		public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
		{
			bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
			{
				new FlavorTextBestiaryInfoElement(
					"")
			});
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			return spawnInfo.Player.Macrocosm().ZoneMoon && !Main.dayTime ? .08f : 0f;
		}

		public override void OnSpawn(IEntitySource source)
		{
			NPC.frame.Y = NPC.GetFrameHeight() * walkFrames.Start;
		}

		#region Netcode

		// Required until we fix the NetSyncAttribute thing :/
		// The NPC.ai[] array is used by BaseMod ZombieAI()
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write((byte)AI_State);
			writer.Write((byte)AimMode);
			writer.Write((ushort)ShootCooldownCounter);
			writer.Write((ushort)ShootSequenceCounter);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			AI_State = (ActionState)reader.ReadByte();
			AimMode = (AimType)reader.ReadByte();
			ShootCooldownCounter = reader.ReadUInt16();
			ShootSequenceCounter = reader.ReadUInt16();
		}

		#endregion

		public override void AI()
		{
			/* //--- Stuff for debug ---
			Main.NewText("Shoot sequence: " + ShootSequenceCounter);
			Main.NewText("Shoot cooldown: " + ShootCooldownCounter);
			Main.NewText("AI State: " + AI_State.ToString());
			Main.NewText("Frame Index: " + NPC.frame.Y / NPC.GetFrameHeight());
			Main.NewText("Frame cnt: " + NPC.frameCounter);
			Main.NewText("\n\n\n");
			*/

			NPC.spriteDirection = -NPC.direction;

			// Override some behaviours if NPC was just hit 
			if (NPC.justHit)
			{
				ShootCooldownCounter = maxCooldown / 4; // Reset cooldown with lower value
				ShootSequenceCounter = 0; // Reset sequence counter!
				AI_State = ActionState.Walk; // Reset to walk state 

				// Reset shoot animation to first frame (TODO: check if really needed anymore)
				if(AI_State == ActionState.Shoot)
					NPC.frame.Y = NPC.GetFrameHeight() * shootFramesCommon.Start; 
			}

			if (AI_State == ActionState.Walk)
			{
				// Increase gravity a bit
				if (NPC.velocity.Y < 0f)
					NPC.velocity.Y += 0.1f;

				// Base Fighter AI
				Utility.AIZombie(NPC, ref NPC.ai, false, true, 0);

				ShootCooldownCounter--;

				// Enter shoot state if cooldown passed, clear line of sight, enemy is stationary (vertically?)
				if (ShootCooldownCounter <= 0 && Collision.CanHit(NPC, TargetPlayer) && NPC.velocity.Y == 0f)
				{
					ShootCooldownCounter = maxCooldown; // Reset cooldown

					// Set up the first weapon draw frame (did not find a way to do it right in FindFrame)
					NPC.frame.Y = NPC.GetFrameHeight() * shootFramesCommon.Start; 

					AI_State = ActionState.Shoot;
				}
			} 
			else
			{
				// Exit state if line of sight is broken or shoot sequence succeeded
				if (!Collision.CanHit(NPC, TargetPlayer) || ShootSequenceCounter >= maxShootSequence)
				{
					ShootSequenceCounter = 0;
					AI_State = ActionState.Walk;
					return;
				}

				ShootSequenceCounter++;

				// Freeze horizontal movement
				NPC.velocity.X *= 0.5f;

				// Orient the NPC towards the player 
				NPC.direction = Main.player[NPC.target].position.X < NPC.position.X ? -1 : 1;

				int projDamage = 100;
				int projType = ProjectileID.BulletDeadeye; // TODO: some dedicated bullet?
				float projSpeed = 120;

				// Aim at target
				Vector2 aimPosition = new(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);

				float aimSpeedX = TargetPlayer.position.X + (float)TargetPlayer.width * 0.5f - aimPosition.X;
				float aimSpeedY = TargetPlayer.position.Y + (float)TargetPlayer.height * 0.5f - aimPosition.Y;

				aimSpeedX += (float)Main.rand.Next(-40, 41) * 0.2f;
				aimSpeedY += (float)Main.rand.Next(-40, 41) * 0.2f;

				float aimLenght = (float)Math.Sqrt(aimSpeedX * aimSpeedX + aimSpeedY * aimSpeedY);
				NPC.netUpdate = true;
				aimLenght = projSpeed / aimLenght;
				aimSpeedX *= aimLenght;
				aimSpeedY *= aimLenght;

				aimPosition.X += aimSpeedX;
				aimPosition.Y += aimSpeedY;

				Vector2 aimVelocity = new(aimSpeedX, aimSpeedY);

				// Shoot (TODO: align projectile and particle by angle)
				if (Main.netMode != NetmodeID.MultiplayerClient && ShootSequenceCounter == sequenceShoot)
				{
 					Projectile.NewProjectile(NPC.GetSource_FromAI(), aimPosition.X, aimPosition.Y, aimSpeedX, aimSpeedY, projType, projDamage, 0f, Main.myPlayer);
					Particle.CreateParticle<DesertEagleFlash>(NPC.Center + aimVelocity * 0.24f, aimVelocity * 0.05f, aimVelocity.ToRotation(), 1f);
					//NPC.velocity.X = 4f * -NPC.direction;
				}
 
				// Get shoot angle
				if (Math.Abs(aimSpeedX) > Math.Abs(aimSpeedY) * 2f)
					AimMode = AimType.Horizontal;
 				else if (aimSpeedY > 0f)
					AimMode = AimType.Downwards;
 				else
					AimMode = AimType.Upwards;
			}
		}

		// Animation. 
		public override void FindFrame(int frameHeight)
		{
			int frameIndex = NPC.frame.Y / frameHeight;

			// If not airborne
			if (NPC.velocity.Y == 0f)
			{
				// Walking animation 
				if(AI_State == ActionState.Walk)
				{
					// Reset walking 
					if(!walkFrames.Contains(frameIndex))
						NPC.frame.Y = frameHeight * walkFrames.Start;

					// Walking animation frame counter, accounting for walk speed
					NPC.frameCounter += Math.Abs(NPC.velocity.X) * 2f;

					// Update frame
					if (NPC.frameCounter > 6.0)
					{
						NPC.frame.Y += frameHeight;
						NPC.frameCounter = 0.0;
					}

					if (frameIndex >= walkFrames.End)
 						NPC.frame.Y = frameHeight * walkFrames.Start;
 				}
				else if(AI_State == ActionState.Shoot)
				{
					NPC.frameCounter += 1f;

					// Speed up animation for recoil animation  
					if(ShootSequenceCounter > visualShoot)
						NPC.frameCounter += 0.2f;

					// Update frame 
					if (NPC.frameCounter > 6.0)
					{
						// After weapon draw animation, sitck to an aim frame, based on the aim angle
						if (frameIndex > shootFramesCommon.End && ShootSequenceCounter <= visualShoot)
						{
							switch (AimMode)
							{
								case AimType.Horizontal:
									NPC.frame.Y = frameHeight * shootFramesHorizontal.Start;
									break;

								case AimType.Upwards:
									NPC.frame.Y = frameHeight * shootFramesUpwards.Start;
									break;

								case AimType.Downwards:
									NPC.frame.Y = frameHeight * shootFramesDownward.Start;
									break;
							}
						}
						else // Update the frame
						{
							NPC.frame.Y += frameHeight;
						}

						NPC.frameCounter = 0.0;
					}
 				}
 			}
			// Air-borne frame
			else if(NPC.velocity.Y > 1f)
			{
				NPC.frameCounter = 0.0;
				NPC.frame.Y = frameHeight * fallingFrame;
			}
		}

		public override void ModifyNPCLoot(NPCLoot loot)
		{
		}

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life > 0)
			{
				for (int i = 0; i < 30; i++)
				{
					int dustType = Utils.SelectRandom<int>(Main.rand, DustID.TintableDust, DustID.Blood);

 					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, dustType);
					dust.velocity.X *= (dust.velocity.X + + Main.rand.Next(0, 100) * 0.015f) * hit.HitDirection;
					dust.velocity.Y =  3f + Main.rand.Next(-50, 51) * 0.01f  ;
					dust.scale *= 1f + Main.rand.Next(-30, 31) * 0.01f;
					dust.noGravity = true;
				}
			}

			// Don't spawn gores on dedicated server
			if (Main.dedServ)
 				return; 
 
			if (NPC.life <= 0)
			{
				var entitySource = NPC.GetSource_Death();

				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityHead").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityArm").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityArm").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityLeg1").Type);
				Gore.NewGore(entitySource, NPC.position, NPC.velocity, Mod.Find<ModGore>("ZombieSecurityLeg2").Type);
			}
		}
	}
}
