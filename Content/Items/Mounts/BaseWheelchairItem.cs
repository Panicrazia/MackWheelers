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

namespace MackWheelers.Content.Items.Mounts
{
    internal class BaseWheelchairItem : ModItem
    {
        public Dictionary<WheelchairAccessoryTypeEnum, List<Item>> wheelchairAccessoryList = new Dictionary<WheelchairAccessoryTypeEnum, List<Item>>();

        public WheelchairType wheelchairType = WheelchairType.Electric;
        public WheelchairAccessoryTypeEnum enumType = WheelchairAccessoryTypeEnum.Wheelchair;

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
            SetUpAccessorySlots();
        }

        public void SetUpAccessorySlots()
        {
            wheelchairAccessoryList.Add(WheelchairAccessoryTypeEnum.Wheels, new List<Item>());
            wheelchairAccessoryList.Add(WheelchairAccessoryTypeEnum.Box, new List<Item>());
            wheelchairAccessoryList.Add(WheelchairAccessoryTypeEnum.Sidearm, new List<Item>());
            wheelchairAccessoryList.Add(WheelchairAccessoryTypeEnum.Shoulder, new List<Item>());
            wheelchairAccessoryList.Add(WheelchairAccessoryTypeEnum.Hull, new List<Item>());
            wheelchairAccessoryList.Add(WheelchairAccessoryTypeEnum.Undercarriage, new List<Item>());
            wheelchairAccessoryList.Add(WheelchairAccessoryTypeEnum.Back, new List<Item>());
            wheelchairAccessoryList.Add(WheelchairAccessoryTypeEnum.Engine, new List<Item>());
        }

        public bool AddItem(Item item)
        {
            //if(item.ModItem.Type != ModContent.ItemType<BaseWheelchairAcc>())
            if (!(item.ModItem is BaseWheelchairAcc))
            {
                return false;
            }
            else
            {
                WheelchairAccessoryTypeEnum accType = (item.ModItem as BaseWheelchairAcc).GetAccType();
                if (wheelchairAccessoryList[accType].Count >= WheelchairAccMaxValues.ItemSlotsMaxs[accType])
                {
                    Main.NewText("tried to add a slot but there was no space, in basewheelchairitem");
                    Main.NewText("yeah i dunno bruv go fuk urself lmao, in basewheelchairitem");
                    return false;
                }
                else
                {
                    wheelchairAccessoryList[accType].Add(item);
                }
            }

            return true;
        }

        public bool RemoveItem(Item item)
        {
            //TODO:
            //go through the list and find the item, then remove it

            /*
            if (item.ModItem.Type != ModContent.ItemType<BaseWheelchairAcc>())
            {
                return false;
            }
            else
            {
                WheelchairAccessoryTypeEnum accType = (item.ModItem as BaseWheelchairAcc).GetAccType();
                if (wheelchairAccessoryList[accType].Count >= WheelchairAccMaxValues.ItemSlotsMaxs[accType])
                {
                    return false;
                }
                else
                {
                    wheelchairAccessoryList[accType].Add(item);
                }
            }
            */
            return true;
        }


        public override bool AltFunctionUse(Player player)
        {
            ModContent.GetInstance<WheelchairUISystem>().ShowMyUI();
            return true;
        }

        public override ModItem Clone(Item item)
        {
            BaseWheelchairItem clone = (BaseWheelchairItem)base.Clone(item);
            clone.wheelchairType = wheelchairType;
            clone.wheelchairAccessoryList = wheelchairAccessoryList;
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
            List<int> keyIntegers = wheelchairAccessoryList.Keys.Select(key => (int)key).ToList();

            tag["wheelchairAccsKeys"] = keyIntegers;
            tag["wheelchairAccsValues"] = wheelchairAccessoryList.Values.ToList();
            tag["wheelchairType"] = ((int)wheelchairType);
        }

        public override void LoadData(TagCompound tag)
        {
            var names = tag.Get<List<int>>("wheelchairAccsKeys");
            List<WheelchairAccessoryTypeEnum> realNames = names.Select(x => (WheelchairAccessoryTypeEnum)x).ToList();
            var values = tag.Get<List<List<Item>>>("wheelchairAccsValues");
            wheelchairAccessoryList = realNames.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);

            /*
            if (tag.ContainsKey("wheelchairAccs"))
            {
                Dictionary<WheelchairAccessoryTypeEnum, List<Item>> myVariable = tag.Get<Dictionary<WheelchairAccessoryTypeEnum, List<Item>>>("wheelchairAccs") ?? null;
                if (myVariable != null)
                {
                    wheelchairAccessoryList = myVariable;
                }
            }
            */

            //might have to do the same with the below
            wheelchairType = (WheelchairType)tag.Get<int>("wheelchairType");
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {

        }
    }
}