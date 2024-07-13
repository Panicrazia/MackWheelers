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
using System.IO;
using MackWheelers.Content.Items.WheelchairAccessories;

namespace MackWheelers.Content.Items.Mounts
{
    /*
    public class WheelchairAccData : TagSerializable
    {
        public Dictionary<WheelchairAccessoryTypeEnum, List<Item>> wheelchairAccessoryList = new Dictionary<WheelchairAccessoryTypeEnum, List<Item>>();

        public WheelchairAccData()
        {
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

        public TagCompound SerializeData()
        {
            List<int> keyIntegers = wheelchairAccessoryList.Keys.Select(key => (int)key).ToList();
            TagCompound tag = new TagCompound();

            tag["wheelchairAccsKeys"] = keyIntegers;
            tag["wheelchairAccsValues"] = wheelchairAccessoryList.Values.ToList();

            return tag;
        }

        public Dictionary<WheelchairAccessoryTypeEnum, List<Item>> Deserialize(TagCompound tag)
        {

            var names = tag.Get<List<int>>("wheelchairAccsKeys");
            List<WheelchairAccessoryTypeEnum> realNames = names.Select(x => (WheelchairAccessoryTypeEnum)x).ToList();
            var values = tag.Get<List<List<Item>>>("wheelchairAccsValues");
            wheelchairAccessoryList = realNames.Zip(values, (k, v) => new { Key = k, Value = v }).ToDictionary(x => x.Key, x => x.Value);

            return wheelchairAccessoryList;
        }
    }
    */
    internal class BaseWheelchairItem : ModItem
    {
        //public WheelchairAccData wheelchairAccData = new WheelchairAccData();

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

        public Dictionary<WheelchairAccessoryTypeEnum, List<Item>> GetWheelchairAccessoryList()
        {
            return wheelchairAccessoryList;
        }

        /*
            write how many items are in the list
            then use ItemIO.Send to write the items
            then, in the receiving end,
            clear the list
            read the number for how many items there are
            then use ItemIO.Receive to read them

            AND THE MOST IMPORTANT PART:
            WHEN YOU NEED TO SYNC, CALL 
            Item.NetStateChanged(); 
            
         */
        public override void NetSend(BinaryWriter writer)
        {
            //kivi is key value
            List<KeyValuePair<WheelchairAccessoryTypeEnum, List<Item>>> kivis = new List<KeyValuePair<WheelchairAccessoryTypeEnum, List<Item>>>();
            foreach (var kivi in wheelchairAccessoryList)
            {
                kivis.Add(kivi);
            }

            foreach (var kivi in kivis)
            {
                writer.Write((int)kivi.Key);
                writer.Write(kivi.Value.Count);
                foreach(var val in kivi.Value)
                {
                    ItemIO.Send(val, writer);
                }
            }
        }

        public override void NetReceive(BinaryReader reader)
        {
            List<KeyValuePair<WheelchairAccessoryTypeEnum, List<Item>>> kivis = new List<KeyValuePair<WheelchairAccessoryTypeEnum, List<Item>>>();

            wheelchairAccessoryList.Clear();
            int slots;
            for (int i = 0; i < 8; i++)
            {
                WheelchairAccessoryTypeEnum accEnum = (WheelchairAccessoryTypeEnum)reader.ReadInt32();
                List<Item> newList = new List<Item>();
                slots = reader.ReadInt32();
                for (int f = 0; f < slots; f++)
                {
                    newList.Add(ItemIO.Receive(reader));
                }
                wheelchairAccessoryList.Add(accEnum, newList);
            }
            //should work tm
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
                    //this shouldnt ever be called if addItem and removeItem are called properly
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
            if (!(item.ModItem is BaseWheelchairAcc))
            {
                Main.NewText("tried to move item but its not a wheelchairacc how did you do that");
                return false;
            }
            else
            {
                WheelchairAccessoryTypeEnum accType = (item.ModItem as BaseWheelchairAcc).GetAccType();
                if (!wheelchairAccessoryList[accType].Remove(item))
                {
                    Main.NewText("tried to remove item but its not present in the list");
                    return false;
                }
            }
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