﻿using Macrocosm.Common.Utils;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Macrocosm.Content.Dusts
{
    public class GreenBrightDust : ModDust
    {
        public override Color? GetAlpha(Dust dust, Color lightColor) => Color.White.WithAlpha((byte)dust.alpha);

        public override void OnSpawn(Dust dust)
        {
            dust.alpha = 0;
        }

        public override bool Update(Dust dust)
        {
            if (!dust.noGravity)
                dust.velocity.Y += 0.05f;

            if (dust.customData != null && dust.customData is NPC npc)
            {
                dust.position += npc.position - npc.oldPos[1];
            }
            else if (dust.customData != null && dust.customData is Player player)
            {
                dust.position += player.position - player.oldPosition;
            }
            else if (dust.customData != null && dust.customData is Vector2)
            {
                Vector2 vector = (Vector2)dust.customData - dust.position;
                if (vector != Vector2.Zero)
                    vector.Normalize();

                dust.velocity = (dust.velocity * 4f + vector * dust.velocity.Length()) / 5f;
            }

            if (dust.alpha < 127)
                dust.alpha += 10;
            else
                dust.alpha = 127;

            Lighting.AddLight(dust.position, new Color(30, 255, 105).ToVector3() * 0.6f);

            dust.position += dust.velocity;
            dust.rotation += 0.25f * (dust.dustIndex % 2 == 0 ? -1 : 1);
            dust.scale -= 0.08f;
            dust.velocity *= 0.96f;

            if (dust.scale <= 0f)
                dust.active = false;

            return false;
        }
    }
}