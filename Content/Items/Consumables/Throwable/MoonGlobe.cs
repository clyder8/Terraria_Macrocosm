﻿using Macrocosm.Content.Projectiles.Friendly.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Consumables.Throwable
{
	public class MoonGlobe : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moon Globe");
			Tooltip.SetDefault("Toss it to change how the Moon looks!");
		}

		public override void SetDefaults()
		{
			Item.DefaultToThrownWeapon(ModContent.ProjectileType<MoonGlobeProjectile>(), 20, 8f);
			Item.UseSound = SoundID.Item106;
			Item.width = 18;
			Item.height = 18;
			Item.value = Item.sellPrice(gold: 15);
			Item.rare = ItemRarityID.Green;
		}
	}
}
