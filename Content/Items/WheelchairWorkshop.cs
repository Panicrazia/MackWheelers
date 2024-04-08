using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MackWheelers.Content.Items
{
    internal class WheelchairWorkshop : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.WheelchairWorkshop>());

            Item.width = 26;
            Item.height = 22;
            Item.value = 500;
        }

        public override void AddRecipes()
        {
            //TODO

        }
    }

}
