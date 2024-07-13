using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MackWheelers.Content.Mounts
{
    internal class WoodenWheelchairMount : BaseWheelchairMount
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            MountData.buff = ModContent.BuffType<Buffs.WoodenWheelchairMountBuff>();
            MountData.spawnDust = DustID.WoodFurniture;
        }
    }
}
