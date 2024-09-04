﻿using Macrocosm.Common.Utils;
using Macrocosm.Content.Items.Bars;
using Macrocosm.Content.Projectiles.Friendly.Melee;
using Macrocosm.Content.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Weapons.Melee
{
    public class RocheChakram : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.SkipsInitialUseSound[Type] = true;
            
        }
        public override string Texture => ModContent.GetInstance<RocheChakramProjectile>().Texture;
        public override void SetDefaults()
        {
            Item.rare = ModContent.RarityType<MoonRarityT2>();
            Item.value = Item.sellPrice(silver: 4);
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.consumable = true;
            Item.UseSound = SoundID.Item71;
            Item.autoReuse = true;
            Item.damage = 300;
            Item.knockBack =0.5f;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noMelee = true;
            Item.shootSpeed = 20f;
            Item.shoot = ModContent.ProjectileType<RocheChakramProjectile>();
            Item.maxStack=9999;
        }



      


        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            
            Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RocheChakramProjectile>(), damage, knockback, Main.myPlayer, ai0: 0f);
            

            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (!Main.dedServ && Item.UseSound.HasValue)
            {
                SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
            }
            return null;
        }
       
       
    }
}
