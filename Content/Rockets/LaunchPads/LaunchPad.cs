﻿using Macrocosm.Common.Netcode;
using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SubworldLibrary;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Macrocosm.Content.Rockets.LaunchPads
{
	public class LaunchPad : TagSerializable
	{
		[NetSync] public Point16 StartTile;
		[NetSync] public Point16 EndTile;
		[NetSync] public bool HasRocket;

		public int Width => EndTile.X + 1 - StartTile.X;
		public Rectangle Hitbox => new((int)(StartTile.X * 16f), (int)(StartTile.Y * 16f), Width * 16, 16);
		public Vector2 Position => new(((StartTile.X + (EndTile.X - StartTile.X) / 2f) * 16f), StartTile.Y);

		private bool isMouseOver;

		public LaunchPad()
		{
			StartTile = new();
			EndTile = new();
		}

		public LaunchPad(int startTileX, int startTileY, int endTileX, int endTileY)
		{
			StartTile = new(startTileX, startTileY);
			EndTile = new(endTileX, endTileY);
		}

		public LaunchPad(int startTileX, int startTileY) : this(startTileX, startTileY, startTileX, startTileY) { }

		public LaunchPad(Point16 startTile) : this(startTile.X, startTile.Y) { }

		public LaunchPad(Point16 startTile, Point16 endTile) : this(startTile.X, startTile.Y, endTile.X, endTile.Y) { }

		public static LaunchPad Create(string subworldId, int startTileX, int startTileY, int endTileX, int endTileY)
		{
			LaunchPad launchPad = new(startTileX, startTileY, endTileX, endTileY);

			LaunchPadManager.Add(subworldId, launchPad);
			launchPad.NetSync(subworldId);
			return launchPad;
		}

		public static LaunchPad Create(string subworldId, int startTileX, int startTileY) => Create(subworldId, startTileX, startTileY, startTileX, startTileY);
		public static LaunchPad Create(string subworldId, Point16 startTile) => Create(subworldId, startTile.X, startTile.Y);
		public static LaunchPad Create(string subworldId, Point16 startTile, Point16 endTile) => Create(subworldId, startTile.X, startTile.Y, endTile.X, endTile.Y);

		public void Update()
		{
			for(int i = 0; i < RocketManager.MaxRockets; i++)
			{
				Rocket rocket = RocketManager.Rockets[i];

				if (rocket.ActiveInCurrentWorld)
				{
					if (Hitbox.Intersects(rocket.Bounds))
						HasRocket = true;
				}
			}

			isMouseOver = Hitbox.Contains(Main.MouseWorld.ToPoint()) && Hitbox.InPlayerInteractionRange();
		}

		public void Draw(SpriteBatch spriteBatch, Vector2 screenPosition)
		{
			Rectangle rect = Hitbox;
			rect.X -= (int)screenPosition.X; 
			rect.Y -= (int)screenPosition.Y;

			if (isMouseOver)
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, Color.Gold * 0.25f);
		}


		/// <summary>
		/// Syncs the launchpad fields with <see cref="NetSyncAttribute"/> across all clients and the server.
		/// </summary>
		public void NetSync(string subworldId, int ignoreClient = -1)
		{
			if (Main.netMode == NetmodeID.SinglePlayer)
				return;

			ModPacket packet = Macrocosm.Instance.GetPacket();

			if (WriteToPacket(packet, subworldId))
				packet.Send(-1, ignoreClient);

			packet.Dispose();
		}

		public bool WriteToPacket(ModPacket packet, string subworldId)
		{
			packet.Write((byte)MessageType.SyncLaunchPadData);

			packet.Write(subworldId);

			if (this.NetWriteFields(packet)) // Check if the writer was able to write all the fields.
				return true;

			return false;
		}

		/// <summary>
		/// Syncs a rocket with data from the <see cref="BinaryReader"/>. Don't use this method outside <see cref="PacketHandler.HandlePacket(BinaryReader, int)"/>
		/// </summary>
		/// <param name="reader"></param>
		public static void SyncLaunchPadData(BinaryReader reader, int clientWhoAmI)
		{
			string subworldId = reader.ReadString();

			LaunchPad launchPad = new();
			launchPad.NetReadFields(reader);

			LaunchPad existingLaunchPad = LaunchPadManager.GetLaunchPadAtTileCoordinates(subworldId, launchPad.StartTile);
			if (existingLaunchPad is null)
 				LaunchPadManager.Add(subworldId, launchPad);
 
			if (Main.netMode == NetmodeID.Server)
			{
				// Bounce to all other clients, minus the sender
				launchPad.NetSync(subworldId, ignoreClient: clientWhoAmI);

				ModPacket packet = Macrocosm.Instance.GetPacket();
				launchPad.WriteToPacket(packet, subworldId);

				if (SubworldSystem.AnyActive())
					SubworldSystem.SendToMainServer(Macrocosm.Instance, packet.GetBuffer());
				else
					SubworldSystem.SendToAllSubservers(Macrocosm.Instance, packet.GetBuffer());
			}
		}

		public LaunchPad Clone() => DeserializeData(SerializeData());

		public static readonly Func<TagCompound, LaunchPad> DESERIALIZER = DeserializeData;

		public TagCompound SerializeData()
		{
			TagCompound tag = new()
			{
				[nameof(StartTile)] = StartTile,
				[nameof(EndTile)] = EndTile,
				[nameof(HasRocket)] = HasRocket,
			};

			return tag;
		}

		public static LaunchPad DeserializeData(TagCompound tag)
		{
			LaunchPad launchPad = new()
			{
				HasRocket = tag.ContainsKey(nameof(HasRocket)),
			};

			if (tag.ContainsKey(nameof(StartTile)))
				launchPad.StartTile = tag.Get<Point16>(nameof(StartTile));

			if (tag.ContainsKey(nameof(EndTile)))
				launchPad.EndTile = tag.Get<Point16>(nameof(EndTile));

			return launchPad;
		}
	}
}