using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.GameInput;
using MackWheelers.Common.Systems;
using MackWheelers.Content.Mounts;
using Microsoft.CodeAnalysis;
using static Humanizer.In;
using Microsoft.Xna.Framework;
using rail;
using System.IO;
using Terraria.Audio;
using MackWheelers.Content.Items;
using Steamworks;

namespace MackWheelers
{
    public class WheelchairPlayer : ModPlayer
    {
        private Mod mod => Mod;
        public Player player => Player;

        const float DEFAULTwheelchairSlowdownValue = .076f;

        //public bool isCrippled = false;
        public bool isCrippled { get => player.HasBuff(ModContent.BuffType<Content.Buffs.PermanentlyCrippledDebuff>()); }
        public bool isManuallyPushingSelf { get => true; }  //will be used for when the player doesnt have a battery to move themselves smoothly
        public int manualPushingTimer = 0;

        public bool isBeingPushed = false; //is currently in a wheelchair and being pushed by someone
        public bool isPushing = false;  //is currently pushing someone in a wheelchair
        public bool canPassivelyPush = false; //to allow players to have an item in their inventory that lets them keep pushing a wheelchair player while still using items
        public bool isPurposefullyPushing { get => player.HeldItem.IsAir; } //lets players with an empty hand to take a wheelchair player from someone who is pushing them without an empty hand

        public bool canBePushed = true;    //is in a wheelchair and can have someone start pushing them
        public bool canPush = true;     //is able to push someone, only matters with push cooldown, seprately check for if they cant push for other reasons (like holding an item)
        public int pushCooldown = 0;    //when a player stops pushing there is a cooldown before they can push again

        public bool wasRidingWheelchair = false;

        public bool isRidingWheelchair { get => player.mount._type == ModContent.MountType<BaseWheelchairMount>(); }

        //maybe these shouldnt be nullable and just use -1 for nonexistant and check for it?? idk
        public int pusherWhoAmI = -1;
        public WheelchairPlayer pusher { get => pusherWhoAmI != -1 ? Main.player[(int)pusherWhoAmI].GetModPlayer<WheelchairPlayer>() : null; }
        public int playerBeingPushedWhoAmI = -1;
        public WheelchairPlayer playerBeingPushed { get => playerBeingPushedWhoAmI != -1 ? Main.player[(int)playerBeingPushedWhoAmI].GetModPlayer<WheelchairPlayer>() : null; }

        public float mouseAiming;

        public virtual int WheelchairBuff { get => ModContent.BuffType<Content.Buffs.WheelchairMountBuff>(); }

        public bool CanPushPlayers()
        {
            return canPush && (isPurposefullyPushing || isPushing) && player.grapCount <= 0 && !player.mount.Active && !player.DeadOrGhost;
        }

        public override IEnumerable<Item> AddStartingItems(bool mediumCoreDeath)
        {


            return new[] {
                new Item(ModContent.ItemType<A796KiloGramBoulder>(), 1),
            };
        }

        public override void SetControls()
        {
            if (isCrippled && player.controlMount)
            {
                player.RemoveAllGrapplingHooks();
            }
            //should only take place if the player is riding in a wheelchair
            if (isCrippled && !player.mount.Active)
            {
                if (player.grapCount <= 0)
                {
                    player.controlJump = false;
                }

                /*//disabling lrupj so they player effectively cant move
                if (player.controlLeft) 
                {
                    player.direction = -1;
                }
                if (player.controlRight)
                {
                    player.direction = 1;
                }
                player.controlLeft = false;
                player.controlRight = false;
                //player.controlDown = false;
                player.controlUp = false;
                player.controlJump = false;
                */
            }

            if (isRidingWheelchair)
            {
                if (isBeingPushed)
                {
                    player.controlLeft = false;
                    player.controlRight = false;
                    player.controlDown = false;
                    player.controlUp = false;
                    player.controlJump = false;
                    //maybe make it so if they 'dash' then they can break being pushed? if not then just use a hotkey
                }
                if (player.controlHook)
                {
                    player.controlHook = false;
                }
            }
        }
        //both the above and below are primarily for controlling movement, and applying the cripple effect
        public override void PostUpdateRunSpeeds()
        {
            //if I can find a way to make the run animation not spaz out then this is a fine option 
            if (isCrippled && !player.mount.Active)
            {   /* an alternative to just disabling the controls for lrupj
                 * benefits are you can use ropes and drop through platforms without a wheelchair
                 */
                AbsolutelyDestroyMovespeed();
                player.velocity = player.velocity * new Vector2(.9f, 1f);
            }
            else if (player.mount.Active && isRidingWheelchair)
            {
                if (isManuallyPushingSelf)
                {
                    player.runAcceleration = 0f; //IMPORTANT the manual mode will not work if this is not here
                    //player.maxRunSpeed = 0f;
                }
            }
            base.PostUpdateRunSpeeds();
        }

        public void AbsolutelyDestroyMovespeed()
        {
            player.maxRunSpeed = 0.001f;
            player.runAcceleration = 0.1f;
            player.runSlowdown = 0.001f;
            player.moveSpeed = 1f;//if you are a single player masochist make this 0
            player.accRunSpeed = 0f;
            player.dashTime = -1; //no dashes
            player.jump = 0;
            player.jumpBoost = false;
            player.jumpSpeedBoost = 0;
        }

        public override void PreUpdateMovement()
        {
            if (isCrippled && !player.mount.Active)
            {
                //just to be double sure
                AbsolutelyDestroyMovespeed();
            }
            if (player.mount.Active && isRidingWheelchair)
            {
                /* //testing jumps
                player.mount._data.jumpHeight = 5;
                player.mount._data.jumpSpeed = 5f;
                */
                if (isBeingPushed && pusher != null) //if being pushed
                {
                    //Vector2 pusherHand = pusher.player.Center + new Vector2(pusher.player.direction * 0f, 5f);

                    player.direction = pusher.player.direction;

                    //player.velocity = pusher.player.velocity;
                    player.velocity = pusher.player.velocity + new Vector2(pusher.player.direction * 26f, 10f);

                    player.gfxOffY = pusher.player.gfxOffY;

                    //player.forceMerman = true;

                    //player.position = pusher.player.position;
                    player.position = pusher.player.position - pusher.player.velocity;

                    Main.SetCameraLerp(0.1f, 3); //also works with 0,1,2, honestly couldnt really tell a difference

                    //how to make wheelchair not climb staircases from the backend??? please someone tell me
                    /*      maybe its related to checking directionals??? like if you are walking towards a staircase from the back 
                     *      it checks if you are comoing from the back with reading your inputs... 
                     *      so maybe I have to see if I can use the pushing players inputs???
                     *      
                     */
                }
                else //not being pushed, but on wheelchair
                {
                    int groundTile = GetFloorTileID();
                    player.runAcceleration = GetPlayerAccelleration(groundTile);
                    if (isManuallyPushingSelf)
                    {
                        int manualPushingTimerBase = 50;
                        //Main.NewText("player y velocity 2: "+ num3);
                        if (manualPushingTimer <= 0 && groundTile != -1)
                        {
                            //Main.NewText(player.maxRunSpeed);
                            if (player.controlLeft)
                            {
                                player.velocity += new Vector2(-5f * player.runAcceleration, 0f);
                                //Main.NewText("velX = "+ player.velocity.X + "  and runaccel = "+ player.runAcceleration);
                                KneecapSpeed();
                                manualPushingTimer = manualPushingTimerBase;
                                player.controlLeft = false;
                            }
                            else if (player.controlRight)
                            {
                                player.velocity += new Vector2(5f * player.runAcceleration, 0f);
                                //Main.NewText("velX = " + player.velocity.X + "  and runaccel = " + player.runAcceleration);
                                KneecapSpeed();
                                manualPushingTimer = manualPushingTimerBase;
                            }

                            //bro magiluninessence straight up caps your speed at 6 with the wheelchair wtf, even though i let it go as high as 9
                            //
                            //             WHY
                            //
                        }
                    }
                    player.runSlowdown = GetPlayerSlowdown(groundTile);
                    //Main.NewText(player.runSlowdown);


                    if (player.velocity.X > player.runSlowdown)
                    {
                        player.velocity.X -= player.runSlowdown;
                    }
                    else if (player.velocity.X < -player.runSlowdown)
                    {
                        player.velocity.X += player.runSlowdown;
                    }
                }
            }
        }
        public void KneecapSpeed()
        {
            //Main.NewText(player.maxRunSpeed);
            float relativeRunspeed = player.maxRunSpeed + 4; //just to make wheelchairs faster on long periods of flat spaces, most noticeable on ice
            if (player.velocity.X > relativeRunspeed)
            {
                player.velocity.X = relativeRunspeed;
            }
            else if (player.velocity.X < -relativeRunspeed)
            {
                player.velocity.X = -relativeRunspeed;
            }
            player.maxRunSpeed = relativeRunspeed;
        }
        public int GetFloorTileID()
        {
            int num = (int)((player.position.X + (float)(player.width / 2)) / 16f);
            int num2 = (int)((player.position.Y + (float)player.height) / 16f);
            if (player.gravDir == -1f)
            {
                num2 = (int)(player.position.Y - 0.1f) / 16;
            }
            Tile? floorTile = Player.GetFloorTile(num, num2);
            int num3 = -1;
            if (floorTile.HasValue)
            {
                num3 = floorTile.Value.TileType;
            }
            return num3;
        }

        public float GetPlayerSlowdown()
        {
            int id = GetFloorTileID();
            return GetPlayerSlowdown(id);
        }
        public float GetPlayerSlowdown(int ID)
        {
            float returner = DEFAULTwheelchairSlowdownValue;
            if (ID != -1)
            {
                if (TileID.Sets.Falling[ID])
                {
                    returner *= 2f;
                }
                else if (TileID.Sets.Conversion.Snow[ID])
                {
                    returner *= 2f;
                }
                else if (ID == TileID.Mud || ID == TileID.Ash)
                {
                    returner *= 1.5f;
                }
                else if (TileID.Sets.Conversion.Ice[ID])
                {
                    returner *= .09f;
                }
            }

            return returner;
        }

        public float GetPlayerAccelleration()
        {
            int id = GetFloorTileID();
            return GetPlayerAccelleration(id);
        }
        public float GetPlayerAccelleration(int ID)
        {
            float accellerationCoefficient = 1;
            if (ID != -1)
            {
                if (TileID.Sets.Conversion.Sand[ID] || TileID.Sets.Conversion.Snow[ID] || ID == TileID.Mud || ID == TileID.Ash)
                {
                    accellerationCoefficient *= 1f; //if this is 1 it does nothing, otherwise 
                }
                else if (TileID.Sets.Conversion.Ice[ID])
                {
                    accellerationCoefficient *= .35f;
                }
            }

            return accellerationCoefficient;
        }

        public override void PostUpdate()
        {
            //Main.NewText("player y velocity 3: " + player.velocity.Y);
            //Main.NewText("newline");
            if (isBeingPushed && pusher != null)
            {
                //so this stupid stopwatch works
                player.velocity -= new Vector2(pusher.player.direction * 26f, 10f);
                //player.velocity = new Vector2(0, 0);
            }
            if(isManuallyPushingSelf)
            {
                //Main.NewText(player.runSlowdown);
                if (manualPushingTimer > 0) 
                {
                    manualPushingTimer--;
                }
            }
            base.PostUpdate();
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (triggersSet.MouseLeft)
            {

            }
            if (player.mount._type == ModContent.MountType<BaseWheelchairMount>())
            {
                /*
                if (triggersSet.Down)
                {
                    if (player.mount._data.Minecart == true)
                    {
                        player.mount._data.Minecart = false;
                    }
                }
                */
                if (triggersSet.Grapple)
                {
                    //Main.NewText("gets spammed??"); //yes it gets spammed
                    //still gets called, which means I dont actually need a dedicated keybind?
                    //the wheelchair things needs a fire once thing or needs an internal cooldown or something idk
                }
                if (KeybindSystem.WheelchairGrappleKeybind.JustPressed)
                {
                    Main.NewText("uep cheif");
                }

            }


            if (KeybindSystem.WheelchairGrappleKeybind.JustPressed)
            {
                //int buff = Main.rand.Next(BuffID.Count);
                //Player.AddBuff(buff, 600);

                //Main.NewText(triggersSet.KeyStatus.TryAdd("Grapple", false));
                if(player.mount._data != null)
                {
                    if (player.mount._data.Minecart == true)
                    {
                        player.mount._data.Minecart = false;
                    }
                    else
                    {
                        player.mount._data.Minecart = true;
                    }
                }

                Main.NewText("Wheelchair grapplehook go");
            }
        }

        public override void ModifyDrawInfo(ref PlayerDrawSet drawInfo)
        {
            if (player.HasBuff(WheelchairBuff))
            {
                //drawInfo.drawPlayer.legFrame.Y = 336;
                //drawInfo.isBottomOverriden= true;
                drawInfo.isSitting = true;
            }
            if (isPushing)
            {
                player.bodyFrame.Y = player.bodyFrame.Height * 3;
            }
            base.ModifyDrawInfo(ref drawInfo);
        }

        

        //796 kilogram boulder

        public override void PostUpdateBuffs()
        {
            int tileRangeIncrease = 3;
            if (isRidingWheelchair)
            {
                Player.tileRangeX += tileRangeIncrease;
                Player.tileRangeY += tileRangeIncrease;
            }
            base.PostUpdateBuffs();
        }
        /*
        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
        }

        public override void PostUpdateMiscEffects()
        {
            
            base.PostUpdateMiscEffects();
        }
        */
        

        public override void PreUpdate()
        {
            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 mouseDirection = GetMousePosition(player) - player.Center;
                mouseAiming = (float)Math.Atan2(mouseDirection.Y, mouseDirection.X);
            }
            if (pushCooldown > 0)
            {
                pushCooldown--;
                if (pushCooldown <= 0)
                {
                    canPush = true;
                    canBePushed = true;
                }
            }
            else
            {
                //this is here incase for whatever reason a player has one of these variables set to false while not having a cooldown
                if (!canPush || !canBePushed)
                {
                    pushCooldown = 120;
                }
            }
            if (true /* && Main.myPlayer == player.whoAmI*/)
            {
                if (!isPushing)
                {
                    WheelchairPlayer pusheee;
                    if (CanPushPlayers() && (pusheee = GetFirstPushableWheelchairPlayerInRange()) != null)
                    {
                        StartPushing(pusheee, true);
                    }
                }
                else
                {
                    if ((!CanPushPlayers() && !canPassivelyPush) || (!playerBeingPushed.canBePushed || playerBeingPushed.pusher!=this))
                    {
                        StopPushing(true);
                    }
                }
            }
            //this might be too early to check if they should be holding a wheelchair player?
            base.PreUpdate();
        }
        private Vector2 GetMousePosition(Player player)
        {
            Vector2 position = Main.screenPosition;
            position.X += Main.mouseX;
            position.Y += player.gravDir == 1 ? Main.mouseY : Main.screenHeight - Main.mouseY;
            return position;
        }

        public virtual WheelchairPlayer GetFirstPushableWheelchairPlayerInRange()
        {
            foreach (Player playa in Main.player)
            {
                if ((playa.whoAmI != Player.whoAmI) && (playa.Center - player.Center).LengthSquared() < (42f * 42f)) //if player is close enough
                {
                    WheelchairPlayer wheeliePlaya = playa.GetModPlayer<WheelchairPlayer>();
                    if (wheeliePlaya.isRidingWheelchair && wheeliePlaya.canBePushed)
                    {
                        return wheeliePlaya;
                    }
                }
            }
            return null;
        }

        public override void PostItemCheck()
        {
            if(player.HeldItem.IsAir && isPushing)
            {
                if(player.controlUseItem)
                {
                    StopPushingAndThrowWheelchair(true);
                }
            }
        }

        public override void OnRespawn()
        {
            //TODO: if they die in a wheelchair spawn them in a wheelchair
            //same with playerdisconnect/connect
            if (wasRidingWheelchair)
            {
                player.mount.SetMount(ModContent.MountType<Content.Mounts.BaseWheelchairMount>(), this.player);
            }
            base.OnRespawn();
        }
        public override void PlayerConnect()
        {
            base.PlayerConnect();
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            wasRidingWheelchair = isRidingWheelchair;
            StopBothPushing();
            return base.PreKill(damage, hitDirection, pvp, ref playSound, ref genDust, ref damageSource);
        }

        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            //TODO: make dying while pushing a wheelchair cause you to throw them in the opposite direction that you got hit from
            StopBothPushing();
            base.Kill(damage, hitDirection, pvp, damageSource);
        }

        public override void PlayerDisconnect()
        {
            StopBothPushing();
            base.PlayerDisconnect();
        }

        public void StopBothPushing()
        {
            if (isBeingPushed)
            {
                ForceStopBeingPushed(true, false);
            }
            else if (isPushing)
            {
                StopPushing(true);
            }
            //these should never be both on at the same time, if they are it breaks, hopefully this is enough to avoid that
        }

        //unpush when dismounting


        /*
         * nonwheelchair players determine when they start and stop pushing
         * 
         * wheelchair players can send a packet that lets them not be pushed for some amount of time (currently 2 secs (*60 for gameticks))
         * 
         * 
         * Ok so what I need to make it deterministic is just to send when the pushing vars get changed right??
         * that should work fine assuming that the base terraria is syncing correctly and on time
         * 
         * hopefully
         * 
         */

        public void StartPushing(WheelchairPlayer pusheee, bool send = true)
        {
            //Main.NewText("found someone eligiblE?");
            pusheee.isBeingPushed = true;
            playerBeingPushedWhoAmI = pusheee.player.whoAmI;
            playerBeingPushed.pusherWhoAmI = player.whoAmI;
            isPushing = true;

            if (send && Main.netMode == NetmodeID.Server)
            {
                //SendStartPushingPacket(-1, player.whoAmI);
                SendStartPushingPacket(-1, -1);
            }
        }
        public void SendStartPushingPacket(int toWho, int fromWho)
        {
            ModPacket packet = Mod.GetPacket();

            packet.Write((byte)MackWheelers.MessageType.PlayerStartPushing);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)playerBeingPushedWhoAmI); //this shouldnt ever be null if done correctly
            packet.Send(toWho, fromWho);
        }
        public void ReceiveStartPushingPacket(BinaryReader reader, int fromWho)
        {
            int pushedPlayerId = reader.ReadByte();

            isPushing = true;
            canPush = true; //reader.ReadBoolean();
            playerBeingPushedWhoAmI = pushedPlayerId;
            playerBeingPushed.isBeingPushed = true;
            playerBeingPushed.pusherWhoAmI = player.whoAmI;
        }



        public void StopPushing(bool send = true)
        {
            isPushing = false;
            canPush = false;
            pushCooldown = 120;
            if (send && Main.netMode == NetmodeID.Server)
            {
                //SendStopPushingPacket(-1, player.whoAmI);
                SendStopPushingPacket(-1, -1);
            }
            playerBeingPushed.isBeingPushed = false;
            playerBeingPushed.pusherWhoAmI = -1;
            playerBeingPushedWhoAmI = -1;
        }
        //need to always call before playerBeingPushed is set to null, or else you will get the pushed player being fucky
        public void SendStopPushingPacket(int toWho, int fromWho)
        {
            ModPacket packet = Mod.GetPacket();

            packet.Write((byte)MackWheelers.MessageType.PlayerStopPushing);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)playerBeingPushedWhoAmI);
            packet.Send(toWho, fromWho);

        }
        public void ReceiveStopPushingPacket(BinaryReader reader)
        {
            int playerId = reader.ReadByte();

            //this might not be needed if the exception is never triggered
            WheelchairPlayer oneBeingPushed = Main.player[playerId].GetModPlayer<WheelchairPlayer>();

            isPushing = false;
            canPush = false;
            pushCooldown = 120;

            if (playerBeingPushedWhoAmI != -1)
            {
                playerBeingPushed.isBeingPushed = false;
                playerBeingPushed.pusherWhoAmI = -1;
                playerBeingPushedWhoAmI = -1;
            }
        }


        public void StopPushingAndThrowWheelchair(bool send = true)
        {
            ThrowWheelchair(null);

            isPushing = false;
            canPush = false;
            pushCooldown = 120;

            playerBeingPushed.isBeingPushed = false;
            playerBeingPushed.pusherWhoAmI = -1;
            playerBeingPushedWhoAmI = -1;

            if (send)
            {
                //SendStopPushingAndThrowPacket(-1, player.whoAmI);
                SendStopPushingAndThrowPacket(-1, Main.myPlayer);
            }

        }
        public void ThrowWheelchair(float? angle)
        {
            Vector2 mouseDirection;

            if (angle != null) 
            {
                mouseDirection = new Vector2((float)Math.Cos(angle.Value), (float)Math.Sin(angle.Value));
            }
            else
            {
                mouseDirection = new Vector2((float)Math.Cos(mouseAiming), (float)Math.Sin(mouseAiming));
            }
             

            if (mouseDirection.X != 0)
            {
                player.direction = (mouseDirection.X > 0 ? 1 : -1);
            }

            playerBeingPushed.player.velocity = (player.velocity + 10 * mouseDirection);

            //soft caps like this are probs less needed with players
            /*
            var force = playerBeingPushed.player.velocity.Length();
            if (force > 15f)
            {
                playerBeingPushed.player.velocity *= 15f / force;
            }
            */
            SoundEngine.PlaySound(SoundID.Item1 with { Pitch = 1.2f }, player.Center);
        }
        public void SendStopPushingAndThrowPacket(int toWho, int fromWho)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MackWheelers.MessageType.PlayerStopPushingThrow);
            packet.Write((byte)Player.whoAmI);
            packet.Write(mouseAiming);
            /*  TODO:
             * add throwing stuff
             */
            packet.Send(toWho, fromWho);
        }
        public void ReceiveStopPushingAndThrowPacket(BinaryReader reader)
        {
            //TODO: add throwing
            mouseAiming = reader.ReadSingle();

            StopPushingAndThrowWheelchair(false);

            isPushing = false;
            canPush = false;
            pushCooldown = 120;

            if (playerBeingPushedWhoAmI != -1)
            {
                playerBeingPushed.isBeingPushed = false;
                playerBeingPushed.pusherWhoAmI = -1;
                playerBeingPushedWhoAmI = -1;
            }
        }


        //if on a wheelchair it forces people to stop pushing you, and starts a small cooldown before anyone can push you again
        public void ForceStopBeingPushed(bool send = true, bool startcooldown = true)
        {
            if (startcooldown)
            {
                pushCooldown = 120;
            }
            else
            {
                pushCooldown = 3;
            }
            canBePushed = false;
            isBeingPushed = false;
            if (pusher != null)
            {
                pusher.isPushing = false;
                pusher.playerBeingPushedWhoAmI = -1;
                pusherWhoAmI = -1;
            }
            if (send)
            {
                SendForceStopPushingPacket(-1, player.whoAmI);
                //SendForceStopPushingPacket(-1, -1);
            }
        }
        public void SendForceStopPushingPacket(int toWho, int fromWho)
        {
            ModPacket packet = Mod.GetPacket();

            packet.Write((byte)MackWheelers.MessageType.ForceStopPushing);
            packet.Write((byte)Player.whoAmI);

            packet.Send(toWho, fromWho);
        }
        public void ReceiveForceStopPushingPacket(BinaryReader reader)
        {
            ForceStopBeingPushed(false);
        }



        public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
        {
            /*
            ModPacket packet;

            packet = Mod.GetPacket();

            packet.Write((byte)MackWheelers.MessageType.DefaultPlayerSync);
            packet.Write((byte)Player.whoAmI);
            packet.Write((byte)pusherWhoAmI);
            packet.Write((byte)playerBeingPushedWhoAmI);
            packet.Write((bool)isPushing);
            //not sure if the below is needed, including it anyways
            packet.Write((bool)canPush);
            packet.Write((bool)canBePushed);

            //do need a bool for if they are throwing the player, and if so then what direction??
            //not needed in this method
            packet.Send(toWho, fromWho);
            */
        }
        public void ReceiveDefaultSyncPacket(BinaryReader reader, int fromWho)
        {
            pusherWhoAmI = reader.ReadByte();
            playerBeingPushedWhoAmI = reader.ReadByte();
            isPushing = reader.ReadBoolean();

            canPush = reader.ReadBoolean();
            canBePushed = reader.ReadBoolean();
        }


        public override void CopyClientState(ModPlayer targetCopy)
        {
            /*
            WheelchairPlayer clone = (WheelchairPlayer)targetCopy;
            clone.isPushing = isPushing;
            clone.pusherWhoAmI = pusherWhoAmI;
            clone.playerBeingPushedWhoAmI = playerBeingPushedWhoAmI;

            //im not sure if the stuff below this line needs to be synced
            clone.canPush = canPush;
            clone.canBePushed = canBePushed;*/
        }

        public override void SendClientChanges(ModPlayer clientPlayer)
        {
            /*
            WheelchairPlayer clone = (WheelchairPlayer)clientPlayer;
            if (clone.isPushing != isPushing ||
                clone.pusherWhoAmI != pusherWhoAmI ||
                clone.playerBeingPushedWhoAmI != playerBeingPushedWhoAmI ||
                clone.canPush != canPush ||
                clone.canBePushed != canBePushed)
            {
                SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
            }*/
        }
    }
}
