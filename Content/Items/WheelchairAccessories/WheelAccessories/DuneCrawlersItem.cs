using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MackWheelers.Content.Items.WheelchairAccessories.WheelAccessories
{
    internal class DuneCrawlersItem : BaseWheelchairWheelAcc
    {

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.AntlionMandible, 10)
                .AddIngredient(ItemID.Stinger, 10)
                .AddIngredient(ItemID.SlimeBlock, 20)
                //.AddIngredient(ModContent.ItemType<ExampleItem>(), 15)
                .AddTile(ModContent.TileType<Content.Tiles.WheelchairWorkshop>())
                .Register();
        }

        public override WheelchairAccEnum GetAccEnum()
        {
            return WheelchairAccEnum.DuneCrawlers;
        }
    }
}
