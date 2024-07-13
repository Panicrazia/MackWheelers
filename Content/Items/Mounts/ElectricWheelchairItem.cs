using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MackWheelers.Content.Mounts;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
using MackWheelers.Common.UI;
using static System.Formats.Asn1.AsnWriter;
using System.Collections;
using MackWheelers.Content.Items.WheelchairAccessories;

namespace MackWheelers.Content.Items.Mounts
{
    internal class ElectricWheelchairItem : BaseWheelchairItem
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.mountType = ModContent.MountType<ElectricWheelchairMount>();
			wheelchairType = WheelchairType.Electric;
			enumType = WheelchairAccessoryTypeEnum.Wheelchair;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.IronBar, 10)
                .AddIngredient(ItemID.SlimeBlock, 20)
                .AddIngredient(ItemID.Silk, 5)
                //.AddIngredient(ModContent.ItemType<ExampleItem>(), 15)
                .AddTile(ModContent.TileType<Content.Tiles.WheelchairWorkshop>())
                .Register();
        }
    }
}
