using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Macrocosm.Content.Items.Materials.Chunks
{
	public class CinisChunk : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cinis Chunk");
			Tooltip.SetDefault("'The core in the middle holds many universes inside'");
			ItemID.Sets.ItemNoGravity[Type] = true;
			ItemID.Sets.AnimatesAsSoul[Type] = true;
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 6)); // NOTE: TicksPerFrame, Frames
		}

		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 30;
			Item.rare = ItemRarityID.Cyan;
			Item.maxStack = 999;
		}
	}
}