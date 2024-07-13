using MackWheelers.Content.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace MackWheelers.Content.Items.GrappleHooks
{
    internal class PrehmWheelGrapple1Item : BaseWheelGrappleItem
    {
        public override void SetDefaults()
        {
            // Copy values from the Amethyst Hook
            Item.CloneDefaults(ItemID.AmethystHook);
            Item.shootSpeed = 15f; // This defines how quickly the hook is shot.
            Item.shoot = ModContent.ProjectileType<WheelchairGrapple1>(); // Makes the item shoot the hook's projectile when used.

            // If you do not use Item.CloneDefaults(), you must set the following values for the hook to work properly:
            // Item.useStyle = ItemUseStyleID.None;
            // Item.useTime = 0;
            // Item.useAnimation = 0;
        }
        public override void AddRecipes()
        {
            //TODO: make this post skeletron, probably requiring a drop from wheelchair skeletons in the dungeon
            /*
            CreateRecipe()
                .AddRecipeGroup(RecipeGroupID.Wood, 30)
                //.AddIngredient(ModContent.ItemType<ExampleItem>(), 15)
                .AddTile(TileID.WorkBenches)
                .Register();
            */
        }
    }
}
