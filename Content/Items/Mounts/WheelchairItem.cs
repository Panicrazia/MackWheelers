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

namespace MackWheelers.Content.Items.Mounts
{
    internal class WheelchairItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;//SoundID.Item79; // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
            Item.mountType = ModContent.MountType<BaseWheelchairMount>();
        }
        Item[] wheelchairAccessoryList;// = new Item[6];

        public override bool AltFunctionUse(Player player)
        {
            ModContent.GetInstance<WheelchairUISystem>().ShowMyUI();
            return true;
        }

        public override ModItem Clone(Item item)
        {
            WheelchairItem clone = (WheelchairItem)base.Clone(item);
            clone.wheelchairAccessoryList = (Item[])wheelchairAccessoryList?.Clone(); // note the ? here is important, colors may be null if spawned from other mods which don't call OnCreate
            return clone;
        }

        public override void OnCreated(ItemCreationContext context)
        {
            //GenerateNewColors();
        }

        /*
        private void GenerateNewColors()
        {
            colors = new Color[5];
            for (int i = 0; i < 5; i++)
            {
                colors[i] = Main.hslToRgb(Main.rand.NextFloat(), 1f, 0.7f);
            }
        }*/

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            /*
            if (colors == null) //colors may be null if spawned from other mods which don't call OnCreate
                return;

            for (int i = 0; i < colors.Length; i++)
            {
                TooltipLine tooltipLine = new TooltipLine(Mod, "EM" + i, "Example " + i) { OverrideColor = colors[i] };
                tooltips.Add(tooltipLine);
            }*/
        }

        public override void UseAnimation(Player player)
        {
            /*
            if (colors == null)
            {
                GenerateNewColors();
            }
            else
            {
                // cycle through the colors
                colors = colors.Skip(1).Concat(colors.Take(1)).ToArray();
            }*/
        }

        // NOTE: The tag instance provided here is always empty by default.
        // Read https://github.com/tModLoader/tModLoader/wiki/Saving-and-loading-using-TagCompound to better understand Saving and Loading data.
        public override void SaveData(TagCompound tag)
        {
            tag["wheelchairAccs"] = wheelchairAccessoryList;
        }

        public override void LoadData(TagCompound tag)
        {
            wheelchairAccessoryList = tag.Get<Item[]>("wheelchairAccs");
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
        }
    }
}