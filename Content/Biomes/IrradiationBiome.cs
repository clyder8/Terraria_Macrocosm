﻿using Macrocosm.Common.Systems;
using Terraria;
using Terraria.ModLoader;
using Macrocosm.Content.Players;
using Macrocosm.Content.Backgrounds.Moon;
using Microsoft.Xna.Framework;
namespace Macrocosm.Content.Biomes
{
    public class IrradiationBiome : MoonBiome
    {
        public override SceneEffectPriority Priority => base.Priority + 2;

        public override string BestiaryIcon => Macrocosm.TexturesPath + "Icons/Moon";
        public override string BackgroundPath => Macrocosm.TexturesPath + "MapBackgrounds/Moon";
        public override string MapBackground => BackgroundPath;

        public override Color? BackgroundColor => base.BackgroundColor;
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<MoonSurfaceBackgroundStyle>();
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.GetInstance<MoonUndergroundBackgroundStyle>();
        public override int Music => Main.dayTime ? MusicLoader.GetMusicSlot(Mod, "Assets/Music/Deadworld") : MusicLoader.GetMusicSlot(Mod, "Assets/Music/Requiem");

        public override void SetStaticDefaults()
        {
        }

        public override bool IsBiomeActive(Player player) => base.IsBiomeActive(player) && TileCounts.Instance.IrradiatedRockCount > 400;

        public override void OnInBiome(Player player)
        {
        }

        public override void OnEnter(Player player)
        {
        }

        public override void OnLeave(Player player)
        {
        }
    }
}
