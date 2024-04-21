using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MackWheelers.Content.Items.WheelchairAccessories
{
    internal class BaseWheelchairBoxAcc : BaseWheelchairAcc
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            enumType = WheelchairAccessoryTypeEnum.Box;
        }
    }
}
