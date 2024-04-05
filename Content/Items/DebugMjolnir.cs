using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using MackWheelers.Content.Mounts;

namespace MackWheelers.Content.Items
{
    internal class DebugMjolnir : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 30;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing; // how the player's arm moves when using the item
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item79; // What sound should play when using the item
            Item.noMelee = true; // this item doesn't do any melee damage
        }

        public override bool? UseItem(Player player)
        {
            Player buggeryDo = player;
            //player.ApplyItemTime()
            var thingy = player.GetModPlayer<WheelchairPlayer>().CanPushPlayers();
            //Main.NewText();
            
            player.Male = true;
            //player.legFrame.Y = 336;
            return base.UseItem(player);
            return true;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
        }
    }
}