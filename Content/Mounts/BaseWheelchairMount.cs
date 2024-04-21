using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.CodeAnalysis;
using Terraria.GameContent;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using static Mono.CompilerServices.SymbolWriter.CodeBlockEntry;
using MackWheelers.Content.Players;

namespace MackWheelers.Content.Mounts
{
    public class BaseWheelchairMount : ModMount
    {



        // Since only a single instance of ModMountData ever exists, we can use player.mount._mountSpecificData to store additional data related to a specific mount.
        // Using something like this for gameplay effects would require ModPlayer syncing, but this example is purely visual.
        /*
        protected class CarSpecificData
        {
            internal static float[] offsets = new float[] { 0, 14, -14 };

            internal int count; // Tracks how many balloons are still left.
            internal float[] rotations;

            public CarSpecificData()
            {
                count = 3;
                rotations = new float[count];
            }
        }*/

        public override void SetStaticDefaults()
        {
            
            MountData.jumpHeight = 0; // How high the mount can jump.
            MountData.acceleration = 0.1f; // The rate at which the mount speeds up.
            MountData.jumpSpeed = 0f; // The rate at which the player and mount ascend towards (negative y velocity) the jump height when the jump button is pressed.
            MountData.blockExtraJumps = true; // Determines whether or not you can use a double jump (like cloud in a bottle) while in the mount.
            MountData.constantJump = false; // Allows you to hold the jump button down.
            MountData.heightBoost = -10; // Height between the mount and the ground
            MountData.fallDamage = 0.5f; // Fall damage multiplier.
            MountData.runSpeed = 5f; // The speed of the mount
            MountData.dashSpeed = 5f; // The speed the mount moves when in the state of dashing.
            MountData.flightTimeMax = 0; // The amount of time in frames a mount can be in the state of flying.
            MountData.abilityDuration = 20;
            MountData.abilityCooldown = 100;
            MountData.abilityChargeMax = 100;
            //MountData.Minecart = true; 


            // Misc
            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.WheelchairMountBuff>(); // The ID number of the buff assigned to the mount.

            // Effects
            MountData.spawnDust = DustID.Iron; // The ID of the dust spawned when mounted or dismounted.

            // Frame data and player offsets
            MountData.totalFrames = 4; // Amount of animation frames for the mount
            MountData.playerYOffsets = Enumerable.Repeat(6, MountData.totalFrames).ToArray(); // Fills an array with values for less repeating code
            
            MountData.xOffset = -2;
            //MountData.yOffset = -80;
            MountData.yOffset = 3;
            //MountData.playerHeadOffset = 22;
            MountData.bodyFrame = 11;
            
            // Standing
            MountData.standingFrameCount = 0;
            MountData.standingFrameDelay = 0;
            MountData.standingFrameStart = 0;
            // Running
            MountData.runningFrameCount = 4;
            MountData.runningFrameDelay = 12;
            MountData.runningFrameStart = 0;
            // Flying
            MountData.flyingFrameCount = 0;
            MountData.flyingFrameDelay = 0;
            MountData.flyingFrameStart = 0;
            // In-air
            MountData.inAirFrameCount = 1;
            MountData.inAirFrameDelay = 12;
            MountData.inAirFrameStart = 0;
            // Idle
            MountData.idleFrameCount = 4;
            MountData.idleFrameDelay = 12;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            // Swim
            MountData.swimFrameCount = MountData.inAirFrameCount;
            MountData.swimFrameDelay = MountData.inAirFrameDelay;
            MountData.swimFrameStart = MountData.inAirFrameStart;
            
            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
            
        }

        

        public override void AimAbility(Player player, Vector2 mousePosition)
        {
            for (int i = 0; i < 69; i++)
            {
                Rectangle rect = player.getRect();
                Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Torch);
            }

            //Rectangle rect = player.getRect();
            //Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Torch);

            //MountData.Minecart = true;
            //MountData.bodyFrame++;
            //Console.WriteLine(MountData.bodyFrame);
        }

        public override void UseAbility(Player player, Vector2 mousePosition, bool toggleOn)
        {
            if(toggleOn)
            {
                Rectangle rect = player.getRect();
                for (int i = 0; i < 69; i++)
                {
                    Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, DustID.Torch);
                }
            }
            
            //MountData.Minecart = true;
            //MountData.bodyFrame++;
            //Console.WriteLine(MountData.bodyFrame);
        }

        public override bool UpdateFrame(Player mountedPlayer, int state, Vector2 velocity)
        {
            //the following line is what causes the second leg to appear, and can be used to
            //make the character appear as though they are sitting on the mount, alongside
            //forcing the player to sit which can be found in the WheelchairPlayer class
            //Massive thanks to jopojelly for this
            mountedPlayer.legFrame.Y = 0;
            return base.UpdateFrame(mountedPlayer, state, velocity);
        }

        public override void UpdateEffects(Player player)
        {

            //player.legFrameCounter = 2.0;
            //player.legFrame.Y = 336;
            /*
            PlayerSittingHelper sittingHelper = new PlayerSittingHelper();
            sittingHelper.isSitting = true;
            player.sitting = sittingHelper;
            */
            //player.legRotation = 45f;
            

            // This code simulates some wind resistance for the balloons.
            /*
            var balloons = (CarSpecificData)player.mount._mountSpecificData;
            float balloonMovementScale = 0.05f;

            for (int i = 0; i < balloons.count; i++)
            {
                ref float rotation = ref balloons.rotations[i]; // This is a reference variable. It's set to point directly to the 'i' index in the rotations array, so it works like an alias here.

                if (Math.Abs(rotation) > MathHelper.PiOver2)
                    balloonMovementScale *= -1;

                rotation += -player.velocity.X * balloonMovementScale * Main.rand.NextFloat();
                rotation = rotation.AngleLerp(0, 0.05f);
            }

            // This code spawns some dust if we are moving fast enough.
            if (Math.Abs(player.velocity.X) > 4f)
            {
                Rectangle rect = player.getRect();

                //Dust.NewDust(new Vector2(rect.X, rect.Y), rect.Width, rect.Height, ModContent.DustType<Dusts.Sparkle>());
            }*/
        }

        
        public override void SetMount(Player player, ref bool skipDust)
        {
            player.GetModPlayer<WheelchairPlayer>().OnWheelchairMountup();
            // When this mount is mounted, we initialize _mountSpecificData with a new CarSpecificData object which will track some extra visuals for the mount.
            //player.mount._mountSpecificData = new CarSpecificData();

            // This code bypasses the normal mount spawning dust and replaces it with our own visual.
            DoDust(player);
            skipDust = true;
        }

        public override void Dismount(Player player, ref bool skipDust)
        {
            var wheelchairPlayer = player.GetModPlayer<WheelchairPlayer>();
            if (player.mount._data.Minecart == true)
            {
                player.mount._data.Minecart = false;
            }
            wheelchairPlayer.StopBothPushing();
            wheelchairPlayer.OnWheelchairMountup();
            DoDust(player);
            skipDust = true;
        }

        public void DoDust(Player player)
        {
            if (!Main.dedServ)
            {
                for (int i = 0; i < 70; i++)
                {
                    int num2 = Dust.NewDust(new Vector2(player.position.X - 20f, player.position.Y), player.width + 40, player.height, 8, 0f, 0f);
                    //Main.dust[num2].scale += (float)Main.rand.Next(-10, 21) * 0.01f;
                }
            }
        }
    }
}
