using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace MackWheelers.Content.Players
{
    internal class WheelchairChargeResourcePlayer : ModPlayer
    {
        // Here we create a custom resource, similar to mana or health.
        // Creating some variables to define the current value of our example resource as well as the current maximum value.
        // We also include a temporary max value, as well as some variables to handle the natural regeneration of this resource.
        public int chargeAmount; // Current value of our example resource
        public const int defaultChargeMax = 100; // Default maximum value of example resource
        public int tempChargeMax; // Buffer variable that is used to reset maximum resource to default value in ResetDefaults().
        public int actualChargeMax; // Maximum amount of our example resource. We will change that variable to increase maximum amount of our resource
        public float chargeRegenRate; // By changing that variable we can increase/decrease regeneration rate of our resource
        internal int chargeRegenTimer = 0; // A variable that is required for our timer
        public static readonly Color HealExampleResource = new(20, 216, 239); // We can use this for CombatText, if you create an item that replenishes chargeAmount.

        // In order to make the Example Resource example straightforward, several things have been left out that would be needed for a fully functional resource similar to mana and health. 
        // Here are additional things you might need to implement if you intend to make a custom resource:
        // - Multiplayer Syncing: The current example doesn't require MP code, but pretty much any additional functionality will require this. ModPlayer.SendClientChanges and CopyClientState will be necessary, as well as SyncPlayer if you allow the user to increase tempChargeMax.
        // - Save/Load permanent changes to max resource: You'll need to implement Save/Load to remember increases to your tempChargeMax cap.
        // - Resource replenishment item: Use GlobalNPC.OnKill to drop the item. ModItem.OnPickup and ModItem.ItemSpace will allow it to behave like Mana Star or Heart. Use code similar to Player.HealEffect to spawn (and sync) a colored number suitable to your resource.

        public override void Initialize()
        {
            tempChargeMax = defaultChargeMax;
        }

        public override void ResetEffects()
        {
            ResetVariables();
        }

        public override void UpdateDead()
        {
            ResetVariables();
        }

        // We need this to ensure that regeneration rate and maximum amount are reset to default values after increasing when conditions are no longer satisfied (e.g. we unequip an accessory that increases our resource)
        private void ResetVariables()
        {
            chargeRegenRate = 1f;
            actualChargeMax = tempChargeMax;
        }

        public override void PostUpdateMiscEffects()
        {
            UpdateResource();
        }

        public override void PostUpdate()
        {
            CapResourceGodMode();
        }

        // Lets do all our logic for the custom resource here, such as limiting it, increasing it and so on.
        private void UpdateResource()
        {
            // For our resource lets make it regen slowly over time to keep it simple, let's use chargeRegenTimer to count up to whatever value we want, then increase currentResource.
            /*
            chargeRegenTimer++; // Increase it by 60 per second, or 1 per tick.

            // A simple timer that goes up to 1 second, increases the chargeAmount by 1 and then resets back to 0.
            if (chargeRegenTimer > 60 / chargeRegenRate)    //this only works for slow charging things, it will not work for my needs
            {
                chargeAmount += 1;
                chargeRegenTimer = 0;
            }
            */
            // Limit chargeAmount from going over the limit imposed by tempChargeMax.
            //maybe have an overcharge mechanic with an accessory?
            chargeAmount = Utils.Clamp(chargeAmount, 0, actualChargeMax);
        }

        /// <summary>
        /// checks if the player should have infinite resources and if so, maxes out charge
        /// </summary>
        private void CapResourceGodMode()
        {
            if (Main.myPlayer == Player.whoAmI && Player.creativeGodMode)
            {
                chargeAmount = actualChargeMax;
            }
        }
    }
}
