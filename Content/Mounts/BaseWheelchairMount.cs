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
using ReLogic.Content;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MackWheelers.Content.Mounts
{
    public class BaseWheelchairMount : ModMount
    {
        //Making this abstract breaks everything for some reason, so thats why its not abstract, if someone knows why it does this then id like to know

        private object wheelchairPlayer;



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
            MountData.acceleration = 0.19f; // The rate at which the mount speeds up.
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
            MountData.buff = ModContent.BuffType<Buffs.BaseWheelchairMountBuff>(); // The ID number of the buff assigned to the mount.

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
        }

        public override void UseAbility(Player player, Vector2 mousePosition, bool toggleOn)
        {
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

        public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        {
            /* //some base terraria mount draw code
            if (playerDrawData == null)
            {
                return;
            }
            //Texture2D texture2D2;
            //Texture2D texture2D;
            switch (drawType)
            {
                case 0:
                    texture2D = this._data.backTexture.Value;
                    texture2D2 = this._data.backTextureGlow.Value;
                    break;
                case 1:
                    texture2D = this._data.backTextureExtra.Value;
                    texture2D2 = this._data.backTextureExtraGlow.Value;
                    break;
                case 2:
                    if (this._type == 0 && this._idleTime >= this._idleTimeNext)
                    {
                        return;
                    }
                    texture2D = this._data.frontTexture.Value;
                    texture2D2 = this._data.frontTextureGlow.Value;
                    break;
                case 3:
                    texture2D = this._data.frontTextureExtra.Value;
                    texture2D2 = this._data.frontTextureExtraGlow.Value;
                    break;
                default:
                    texture2D = null;
                    texture2D2 = null;
                    break;
            }
            int type = this._type;
            if (type == 50 && texture2D != null && texture2D != Asset<Texture2D>.DefaultValue)
            {
                PlayerQueenSlimeMountTextureContent queenSlimeMount = TextureAssets.RenderTargets.QueenSlimeMount;
                queenSlimeMount.Request();
                if (queenSlimeMount.IsReady)
                {
                    texture2D = queenSlimeMount.GetTarget();
                }
            }
            if (texture2D == null)
            {
                return;
            }
            type = this._type;
            if ((type == 0 || type == 9) && drawType == 3 && shadow != 0f)
            {
                return;
            }
            int num = this.XOffset;
            int num6 = this.YOffset + this.PlayerOffset;
            if (drawPlayer.direction <= 0 && (!this.Cart || !this.Directional))
            {
                num *= -1;
            }
            Position.X = (int)(Position.X - Main.screenPosition.X + (float)(drawPlayer.width / 2) + (float)num);
            Position.Y = (int)(Position.Y - Main.screenPosition.Y + (float)(drawPlayer.height / 2) + (float)num6);
            int num7 = 0;
            bool flag = true;
            int num8 = this._data.totalFrames;
            int num9 = this._data.textureHeight;
            int num11 = num9 / num8;
            
            if (flag)
            {
                value.Height -= 2;
            }
            if (MountID.Sets.FacePlayersVelocity[this._type])
            {
                spriteEffects = ((Math.Sign(drawPlayer.velocity.X) == -drawPlayer.direction) ? (playerEffect ^ SpriteEffects.FlipHorizontally) : playerEffect);
            }
            if (MountLoader.Draw(this, playerDrawData, drawType, drawPlayer, ref texture2D, ref texture2D2, ref Position, ref value, ref drawColor, ref color, ref num13, ref spriteEffects, ref origin, ref scale, shadow))
            {
                DrawData item6 = new DrawData(texture2D, Position, value, drawColor, num13, origin, scale, spriteEffects);
                item6.shader = Mount.currentShader;
                playerDrawData.Add(item6);
                if (texture2D2 != null)
                {
                    item6 = new DrawData(texture2D2, Position, value, color * ((float)(int)drawColor.A / 255f), num13, origin, scale, spriteEffects);
                    item6.shader = Mount.currentShader;
                }
                playerDrawData.Add(item6);
            }
            */

            //Texture2D texture = ModContent.Request<Texture2D>("YourModName/Items/MyItem_Glowmask", AssetRequestMode.ImmediateLoad).Value;
            /*
            spriteBatch.Draw
            (
                texture,
                new Vector2
                (
                    item.position.X - Main.screenPosition.X + item.width * 0.5f,
                    item.position.Y - Main.screenPosition.Y + item.height - texture.Height * 0.5f + 2f
                ),
                new Rectangle(0, 0, texture.Width, texture.Height),
                Color.White,
                rotation,
                texture.Size() * 0.5f,
                scale,
                SpriteEffects.None,
                0f
            );*/
            /*
            Texture2D texture2D;
            Rectangle rect;
            DrawData drawThing;
            int frameHeight;
            */
            WheelchairPlayer wheelchairPlayer = drawPlayer.GetModPlayer<WheelchairPlayer>();
            Vector2 totalOffset = GetTotalOffset(wheelchairPlayer);
            //Main.NewText(totalOffset);
            //breakdown of the offset numbers, the -30 is from the difference between the space on the left and the space on the right of a wheelchair part
            //the -22 is the distance between the empty space on the left of the part, combined with the distance of the part graphics that arent covered by the chair

            //Rectangle value = new Rectangle(0, num11 * num7, this._data.textureWidth, num11);


            switch (drawType)
            {
                case 0:
                    if (wheelchairPlayer.wheelAccCrawlers)
                    {
                        DrawCrawlers(true, playerDrawData, drawPlayer, totalOffset, drawPosition, drawColor, rotation, drawOrigin, drawScale, spriteEffects);
                    }
                    if (wheelchairPlayer.wheelAccSleds)
                    {
                        DrawSleds(true, playerDrawData, drawPlayer, totalOffset, drawPosition, drawColor, rotation, drawOrigin, drawScale, spriteEffects);
                    }
                    drawOrigin.Y -= totalOffset.Y;
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    if (wheelchairPlayer.wheelAccSleds)
                    {
                        DrawSleds(false, playerDrawData, drawPlayer, totalOffset, drawPosition, drawColor, rotation, drawOrigin, drawScale, spriteEffects);
                    }
                    if (wheelchairPlayer.wheelAccCrawlers)
                    {
                        DrawCrawlers(false, playerDrawData, drawPlayer, totalOffset, drawPosition, drawColor, rotation, drawOrigin, drawScale, spriteEffects);
                    }
                    break;
                default:
                    break;
            }

            //Main.NewText(drawType + ": " + drawOrigin + " " + totalOffset);
            //drawThing.shader = Mount.currentShader;
            //playerDrawData.Add(drawThing);

            return base.Draw(playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
        }

        public void DrawCrawlers(bool back, List<DrawData> playerDrawData, Player player, Vector2 totalOffset, Vector2 drawPosition, Color drawColor, float rotation, Vector2 drawOrigin, float drawScale, SpriteEffects spriteEffects)
        {
            Vector2 offset = WheelchairAccessoryVisualOffsets.getCrawlerOffset(player);
            Texture2D texture2D = ModContent.Request<Texture2D>("MackWheelers/Content/Items/WheelchairAccessories/WheelAccessories/DuneCrawlers"+(back? "_Back": "_Front"), AssetRequestMode.ImmediateLoad).Value;
            int frameHeight = texture2D.Height / MountData.totalFrames;
            //Main.NewText(texture2D.Height);
            Rectangle rect = new Rectangle(0, (frameHeight * player.mount._frame), texture2D.Width, frameHeight);
            DrawData drawThing = new DrawData(texture2D, drawPosition, rect, drawColor, rotation, (drawOrigin - offset + (back ? Vector2.Zero : totalOffset)), drawScale, spriteEffects);
            drawThing.shader = Mount.currentShader;
            playerDrawData.Add(drawThing);
        }

        public void DrawSleds(bool back, List<DrawData> playerDrawData, Player player, Vector2 totalOffset, Vector2 drawPosition, Color drawColor, float rotation, Vector2 drawOrigin, float drawScale, SpriteEffects spriteEffects)
        {
            Vector2 offset = WheelchairAccessoryVisualOffsets.getSledOffset(player);
            Texture2D texture2D = ModContent.Request<Texture2D>("MackWheelers/Content/Items/WheelchairAccessories/WheelAccessories/SnowSleds" + (back ? "_Back" : "_Front"), AssetRequestMode.ImmediateLoad).Value;
            int frameHeight = texture2D.Height / MountData.totalFrames;
            //Main.NewText(texture2D.Height);
            Rectangle rect = new Rectangle(0, (frameHeight * player.mount._frame), texture2D.Width, frameHeight);
            DrawData drawThing = new DrawData(texture2D, drawPosition, rect, drawColor, rotation, (drawOrigin - offset + (back ? Vector2.Zero : totalOffset)), drawScale, spriteEffects);
            drawThing.shader = Mount.currentShader;
            playerDrawData.Add(drawThing);
        }

        /// <summary>
        /// returns the offset that the entire wheelchair will be raised by
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Vector2 GetTotalOffset(WheelchairPlayer player)
        {
            float maxOffset = 0f;
            float temp;
            if (player.wheelAccCrawlers)
            {
                temp = WheelchairAccessoryVisualOffsets.getCrawlerOffset(player.player).Y;
                if(temp < maxOffset)
                {
                    maxOffset = temp;
                }
            }
            return new Vector2(0f, maxOffset);
        }

        public static class WheelchairAccessoryVisualOffsets
        {
            public static Vector2 getCrawlerOffset(Player player)
            {
                return new Vector2((-22f + (player.direction == -1 ? -30f : 0f)), -2f);
            }
            public static Vector2 getSledOffset(Player player)
            {
                return new Vector2((-22f + (player.direction == -1 ? -30f : 0f)), 0f);
                //return new Vector2(getCrawlerOffsetX(player), getCrawlerOffsetY());
            }
            /*
            public static float getCrawlerOffsetX(Player player)
            {
                return (-22f + (player.direction == -1 ? -30f : 0f));
            }
            public static float getCrawlerOffsetY()
            {
                return -2f;
            }
            */
        }
    }
}
