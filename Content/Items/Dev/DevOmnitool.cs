﻿using Macrocosm.Common.Sets;
using Macrocosm.Common.Systems.Power;
using Macrocosm.Common.Utils;
using Macrocosm.Content.Tiles.Ambient;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Dev
{
    class DevOmnitool : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.staff[Type] = true;
            ItemSets.DeveloperItem[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 40;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.UseSound = SoundID.Item6;
        }


        public override bool? UseItem(Player player)
        {
            CircuitSystem.SearchAllCircuits();
            return true;
        }
    }

}
