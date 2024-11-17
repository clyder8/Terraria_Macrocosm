using Macrocosm.Content.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Macrocosm.Common.Utils;
using Terraria.ID;
using Terraria.ModLoader;
using Macrocosm.Content.Tiles.Ambient;

namespace Macrocosm.Content.Tiles.Blocks.Terrain
{
    public class ProtolithCrystalGrower : ModTile
    {
        /// <summary> Types that merge with this. Use instead of <c>Main.tileFrame[][]</c> </summary>
        public static bool[] TileMerge { get; set; } = TileID.Sets.Factory.CreateBoolSet();
        public override string Texture => "Macrocosm/Content/Tiles/Blocks/Terrain/Protolith";
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;

            TileID.Sets.ChecksForMerge[Type] = true;
            Regolith.TileMerge[Type] = true;
            Protolith.TileMerge[Type] = true;

            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            MinPick = 225;
            MineResist = 3f;
            AddMapEntry(new Color(65, 65, 65));
            HitSound = SoundID.Tink;
            DustType = ModContent.DustType<ProtolithDust>();
            RegisterItemDrop(ModContent.ItemType<Items.Blocks.Terrain.Protolith>(), 0);

        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
        {
            WorldGen.TileMergeAttempt(-2, ModContent.TileType<Regolith>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
            WorldGen.TileMergeAttemptFrametest(i, j, Type, TileMerge, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
        }
        public override void NearbyEffects(int i, int j, bool closer)		
        {
            Tile below = Main.tile[i, j + 1];
            Tile above = Main.tile[i, j - 1];
            Tile right = Main.tile[i + 1, j];
            Tile left = Main.tile[i - 1, j];
            if (!above.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(2))
            {
                int rand = Main.rand.Next(20);
                WorldGen.PlaceObject(i, j - 1, ModContent.TileType<LuminiteCrystalTile>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j - 1, ModContent.TileType<LuminiteCrystalTile>(), rand, 0, -1, -1);
            }
            if (!below.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(2))
            {
                int rand = Main.rand.Next(20);
                WorldGen.PlaceObject(i, j + 1, ModContent.TileType<LuminiteCrystalTile>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i, j + 1, ModContent.TileType<LuminiteCrystalTile>(), rand, 0, -1, -1);
            }
            if (!right.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(2))
            {
                int rand = Main.rand.Next(20);
                WorldGen.PlaceObject(i+1, j, ModContent.TileType<LuminiteCrystalTile>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i+ 1, j, ModContent.TileType<LuminiteCrystalTile>(), rand, 0, -1, -1);
            }
            if (!left.HasTile && Main.tile[i, j].HasTile && Main.rand.NextBool(2))
            {
                int rand = Main.rand.Next(20);
                WorldGen.PlaceObject(i-1, j, ModContent.TileType<LuminiteCrystalTile>(), true, rand);
                NetMessage.SendObjectPlacement(-1, i-1, j, ModContent.TileType<LuminiteCrystalTile>(), rand, 0, -1, -1);
            }
        }
    }
}