using MackWheelers.Content.Buffs;
using MackWheelers.Content.Mounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace MackWheelers.Content.Items 
{
    internal class A796KiloGramBoulder : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
            Item.rare = ItemRarityID.White;
            Item.UseSound = SoundID.DD2_ExplosiveTrapExplode;//SoundID.Item79; // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
            //Item.ToolTip
            Item.maxStack = 1;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
        }

        public override bool? UseItem(Player player)
        {
            if (player.HasBuff(ModContent.BuffType<Buffs.PermanentlyCrippledDebuff>()))
            {
                player.ClearBuff(ModContent.BuffType<Buffs.PermanentlyCrippledDebuff>());
            }
            else
            {
                player.AddBuff(ModContent.BuffType<Buffs.PermanentlyCrippledDebuff>(), 10, true, false);
            }
            //player.GetModPlayer<WheelchairPlayer>().isCrippled = !player.GetModPlayer<WheelchairPlayer>().isCrippled;
            Item.TurnToAir();
            return base.UseItem(player);
        }
    }
}
