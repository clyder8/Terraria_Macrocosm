using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Macrocosm.Content.Tiles.Bars
{
    public class InvarBar : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileShine[Type] = 1100;
            Main.tileSolid[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(58, 58, 58), name);
        }
        public override bool CreateDust(int i, int j, ref int type)
        {
            return false;
        }

    }
}