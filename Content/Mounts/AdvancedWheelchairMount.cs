using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace MackWheelers.Content.Mounts
{
    internal class AdvancedWheelchairMount : BaseWheelchairMount
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            MountData.buff = ModContent.BuffType<Buffs.AdvancedWheelchairMountBuff>();
        }
    }
}
