using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MackWheelers.Content.Items
{
    public static class WheelchairAccMaxValues
    {
        public static Dictionary<WheelchairAccessoryTypeEnum, int> ItemSlotsMaxs = new Dictionary<WheelchairAccessoryTypeEnum, int>();
    }
    internal class WheelchairWheelAcc : ModItem
    {
        public WheelchairAccessoryTypeEnum enumType = WheelchairAccessoryTypeEnum.Wheels;


        public WheelchairAccessoryTypeEnum GetAccType()
        {
            return enumType;
        }
    }

}
