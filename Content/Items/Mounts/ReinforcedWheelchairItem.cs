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
    internal class ReinforcedWheelchairItem : BaseWheelchairItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.mountType = ModContent.MountType<ReinforcedWheelchairMount>();
            wheelchairType = WheelchairType.Reinforced;
            enumType = WheelchairAccessoryTypeEnum.Wheelchair;
        }
        public override void AddRecipes()
        {
            //TODO: make recipie, probably needing a part from wheelchair skeletons in the dungeon
        }
    }
}
