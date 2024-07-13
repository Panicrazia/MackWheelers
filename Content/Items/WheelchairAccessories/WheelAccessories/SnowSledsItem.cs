using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MackWheelers.Content.Items.WheelchairAccessories.WheelAccessories
{
    internal class SnowSledsItem : BaseWheelchairWheelAcc
    {

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 20)
                .AddIngredient(ItemID.FrozenSlimeBlock, 20)
                .AddIngredient(ItemID.CyanPaint, 10)
                .AddIngredient(ItemID.DeepCyanPaint, 10)
                //.AddIngredient(ModContent.ItemType<ExampleItem>(), 15)
                .AddTile(ModContent.TileType<Content.Tiles.WheelchairWorkshop>())
                .Register();
        }

        public override WheelchairAccEnum GetAccEnum()
        {
            return WheelchairAccEnum.SnowSleds;
        }
    }
}
