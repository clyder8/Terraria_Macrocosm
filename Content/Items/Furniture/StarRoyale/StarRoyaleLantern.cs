﻿using Macrocosm.Common.Enums;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Furniture.StarRoyale
{
    public class StarRoyaleLantern : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.Furniture.Luminite.LuminiteLantern>(), (int)LuminiteStyle.StarRoyale);
            Item.width = 14;
            Item.height = 28;
            Item.value = 150;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.StarRoyaleBrick, 6)
                .AddIngredient(ItemID.Torch, 1) // Luminite Crystal
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
