using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MackWheelers.Content.Items.WheelchairAccessories
{
    //TODO: reamke this into base wheelchair acc
    public static class WheelchairAccMaxValues
    {
        public static Dictionary<WheelchairAccessoryTypeEnum, int> ItemSlotsMaxs = new Dictionary<WheelchairAccessoryTypeEnum, int>();
    }

    public enum WheelchairAccEnum
    {
        DuneCrawlers,
        SnowSleds,
        IceSkates,
        Default
    }

    internal abstract class BaseWheelchairAcc : ModItem
    {
        public WheelchairAccessoryTypeEnum enumType;

        public WheelchairAccessoryTypeEnum GetAccType()
        {
            return enumType;
        }

        public virtual WheelchairAccEnum GetAccEnum()
        {
            return WheelchairAccEnum.Default;
        }

    }

}
