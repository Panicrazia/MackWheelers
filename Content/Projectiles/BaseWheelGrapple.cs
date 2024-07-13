using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using ReLogic.Content;
using Microsoft.Xna.Framework.Graphics;
using MackWheelers.Content.Items.GrappleHooks;
using Microsoft.CodeAnalysis;
using MackWheelers.Content.Players;

namespace MackWheelers.Content.Projectiles
{
    public abstract class BaseWheelGrapple : ModProjectile
    {
        public static float hookRange = 250f;

        public static void QuickWheelchairGrapple(Player player)
        {
            if (player.frozen || player.tongued || player.webbed || player.stoned || player.dead)
            {
                return;
            }
            /*  why is this a thing
            if (PlayerInput.GrappleAndInteractAreShared)
            {
                if (Main.HoveringOverAnNPC || Main.SmartInteractShowingGenuine || Main.SmartInteractShowingFake || (this._quickGrappleCooldown > 0 && !Main.mapFullscreen) || (WiresUI.Settings.DrawToolModeUI && PlayerInput.UsingGamepad))
                {
                    return;
                }
                bool num32 = this.controlUseTile;
                bool flag = this.releaseUseTile;
                if (!num32 && !flag)
                {
                    return;
                }
                Tile tileSafely = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
                if ((tileSafely.active() && (tileSafely.type == 4 || tileSafely.type == 33 || tileSafely.type == 372 || tileSafely.type == 174 || tileSafely.type == 646 || tileSafely.type == 49)) || (this.inventory[this.selectedItem].type == 3384 && PlayerInput.UsingGamepad))
                {
                    return;
                }
            }*/
            if (player.noItems)
            {
                return;
            }
            /*  this statement is the entire reason I have to copy this whole thing
            if (player.mount.Active)
            {
                player.mount.Dismount(player);
            }*/
            Item item = QuickWheelGrapple_GetItemToUse(player);
            if(item == null)
            {
                return;
            }
            //bool? flag3 = ProjectileLoader.CanUseGrapple(item.shoot, player); //fucks it up for some reason
            bool? flag3 = ProjectileLoader.GetProjectile(item.shoot)?.CanUseGrapple(player);
            if (flag3.HasValue)
            {
                if (!flag3.GetValueOrDefault())
                {
                    item = null;
                }
            }
            if (item == null)
            {
                return;
            }
            player.UpdateBlacklistedTilesForGrappling();
            //SoundEngine.PlaySound(item.UseSound, player.position);

            //I dont know what this is syncing tbh, the sound maybe? idk
            if (Main.netMode == 1 && player.whoAmI == Main.myPlayer)
            {
                NetMessage.SendData(51, -1, -1, null, player.whoAmI, 2f);
            }

            int projectile = item.shoot;
            float shootSpeed = item.shootSpeed;
            int damage = item.damage;
            float knockBack = item.knockBack;

            //DeleteOtherAttatchedHooks(player);

            Vector2 vector = new Vector2(player.position.X + (float)player.width * 0.5f, player.position.Y + (float)player.height * 0.5f);
            float num22 = (float)Main.mouseX + Main.screenPosition.X - vector.X;
            float num23 = (float)Main.mouseY + Main.screenPosition.Y - vector.Y;
            if (player.gravDir == -1f)
            {
                num23 = Main.screenPosition.Y + (float)Main.screenHeight - (float)Main.mouseY - vector.Y;
            }
            float num24 = (float)Math.Sqrt(num22 * num22 + num23 * num23);
            if ((float.IsNaN(num22) && float.IsNaN(num23)) || (num22 == 0f && num23 == 0f))
            {
                num22 = player.direction;
                num23 = 0f;
                num24 = shootSpeed;
            }
            else
            {
                num24 = shootSpeed / num24;
            }
            num22 *= num24;
            num23 *= num24;
            Projectile.NewProjectile(new EntitySource_ItemUse(player, item), vector.X, vector.Y, num22, num23, projectile, damage, knockBack, player.whoAmI);
        }

        //this needs to change to be correct
        public static Item QuickWheelGrapple_GetItemToUse(Player player)
        {
            Item item = null;
            if (Main.projHook[player.miscEquips[4].shoot])
            {
                item = player.miscEquips[4];
                if(!(item.ModItem is BaseWheelGrappleItem))
                {
                    item = null;
                }
            }
            return item;
        }

        private static Asset<Microsoft.Xna.Framework.Graphics.Texture2D> chainTexture;

        public override void Load()
        { // This is called once on mod (re)load when this piece of content is being loaded.
          // This is the path to the texture that we'll use for the hook's chain. Make sure to update it.
            chainTexture = ModContent.Request<Texture2D>("MackWheelers/Content/Projectiles/WheelchairGrappleChain1");
        }


        public override void SetDefaults()
        {
            // These are copied through the CloneDefaults method
            /*
            Projectile.netImportant = true;
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = 7;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft *= 10;
            */
            Projectile.CloneDefaults(ProjectileID.StaticHook);

            Projectile.width = 26;
            Projectile.height = 26;
            //DrawOriginOffsetY = -8; // Adjusts the draw position
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Decide color of the pole by getting the index of a random entry from the PossibleLineColors array.
        }

        public override bool PreAI()
        {
            return false;
        }

        public override void PostAI()
        {
            GrappleHookAI();
        }

        //never gets called because of preai
        public override void AI()
        {
            // Always ensure that graphics-related code doesn't run on dedicated servers via this check.
            if (!Main.dedServ)
            {
                // Create some light based on the color of the line.
                //Lighting.AddLight(Projectile.Center, FishingLineColor.ToVector3());
            }
        }

        //return true if you want to grapple, false otherwise
        public override bool? CanUseGrapple(Player player)
        {
            var temp = player.GetModPlayer<WheelchairPlayer>();
            if (temp != null && temp.isRidingWheelchair)
            {
                int hooksOut = 0;
                foreach (var projectile in Main.ActiveProjectiles)
                {
                    if (projectile.owner == Main.myPlayer && projectile.type == Projectile.type)
                    {
                        hooksOut++;
                    }
                }

                return hooksOut <= 1;
            }
            else
            {
                return false;
            }
            //return base.CanUseGrapple(player);
        }

        //use to kill old tiles
        public void DeleteOtherAttatchedHooks(Player player)
        {
            //loop through all projectiles of given type like so
            int num13 = 0;
            int num14 = -1;
            int num15 = 100000;
            for (int num16 = 0; num16 < 1000; num16++)
            {
                if (Main.projectile[num16].active && Main.projectile[num16].owner == player.whoAmI && Main.projectile[num16].type == this.Type)
                {
                    num13++;
                    if (Main.projectile[num16].timeLeft < num15)
                    {
                        num14 = num16;
                        num15 = Main.projectile[num16].timeLeft;
                    }
                }
            }
            if (num13 > 1)
            {
                Main.projectile[num14].Kill();
            }
            //base.DeleteOtherAttatchedHooks(player, ref type);
        }

        //use to kill old tiles
        public static void DeleteAllHooks(Player player)
        {
            //loop through all projectiles of given type like so
            for (int num16 = 0; num16 < 1000; num16++)
            {
                if (Main.projectile[num16].active && Main.projectile[num16].owner == player.whoAmI && Main.projectile[num16].ModProjectile is BaseWheelGrapple)
                {
                    Main.projectile[num16].Kill();
                }
            }
        }

        private void GrappleHookAI()
        {
            Projectile.timeLeft = 10;
            Projectile.aiStyle = 0;
            if (Main.player[Projectile.owner].dead || Main.player[Projectile.owner].stoned || Main.player[Projectile.owner].webbed || Main.player[Projectile.owner].frozen)
            {
                Projectile.Kill();
                return;
            }
            Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 grappleCenter = new Vector2(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
            float xDistanceFromPlayer = mountedCenter.X - grappleCenter.X;
            float yDistanceFromPlayer = mountedCenter.Y - grappleCenter.Y;
            float distanceFromPlayer = (float)Math.Sqrt(xDistanceFromPlayer * xDistanceFromPlayer + yDistanceFromPlayer * yDistanceFromPlayer);
            Projectile.rotation = (float)Math.Atan2(yDistanceFromPlayer, xDistanceFromPlayer) - 1.57f;

            //cracking brittle dungeon tiles
            if (Main.myPlayer == Projectile.owner)
            {
                int playerGameX = (int)(Projectile.Center.X / 16f);
                int playerGameY = (int)(Projectile.Center.Y / 16f);
                if (playerGameX > 0 && playerGameY > 0 && playerGameX < Main.maxTilesX && playerGameY < Main.maxTilesY && Main.tile[playerGameX, playerGameY].HasUnactuatedTile && TileID.Sets.CrackedBricks[Main.tile[playerGameX, playerGameY].TileType] && Main.rand.Next(16) == 0)
                {
                    WorldGen.KillTile(playerGameX, playerGameY);
                    if (Main.netMode != 0)
                    {
                        NetMessage.SendData(17, -1, -1, null, 20, playerGameX, playerGameY);
                    }
                }
            }

            //high distance cull
            if (distanceFromPlayer > 2500f)
            {
                Projectile.Kill();
            }

            //static hook animation (will be useful since hardmode wheelchair grapple will look like static hook)
            if (Projectile.type == 652 && ++Projectile.frameCounter >= 7)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = 0;
                }
            }

            /* 
             * ai[0] being 0 is going out and trying to latch to tiles
             * ai[0] being 1 is retracting
             * ai[0] being 2 is being attatched to a block
             * 
             */
            if (Projectile.ai[0] == 0f)
            {
                //autoretracting
                //TODO: make maxrange and retract speed variables in the class
                float maxRange = GrappleRange();
                if (distanceFromPlayer > maxRange)
                {
                    Projectile.ai[0] = 1f;
                }

                //making it stay in bounds
                Vector2 hitboxTopLeftCorner = Projectile.Center - new Vector2(5f);
                Vector2 hitboxBottomRightCorner = Projectile.Center + new Vector2(5f);
                Point gameHitboxTopLeftCorner = (hitboxTopLeftCorner - new Vector2(16f)).ToTileCoordinates();
                Point gameHitboxBottomRightCorner = (hitboxBottomRightCorner + new Vector2(32f)).ToTileCoordinates();
                int topLeftCornerX = gameHitboxTopLeftCorner.X;
                int bottomRightCornerX = gameHitboxBottomRightCorner.X;
                int topLeftCornerY = gameHitboxTopLeftCorner.Y;
                int bottomRightCornerY = gameHitboxBottomRightCorner.Y;
                if (topLeftCornerX < 0)
                {
                    topLeftCornerX = 0;
                }
                if (bottomRightCornerX > Main.maxTilesX)
                {
                    bottomRightCornerX = Main.maxTilesX;
                }
                if (topLeftCornerY < 0)
                {
                    topLeftCornerY = 0;
                }
                if (bottomRightCornerY > Main.maxTilesY)
                {
                    bottomRightCornerY = Main.maxTilesY;
                }

                Player player = Main.player[Projectile.owner];
                List<Point> points = new List<Point>();

                //for each hook gets a 5x5 area around them if connected to minecart tracks/plats and puts it in points,
                //this points list is later used to blacklist grappling onto these tiles,
                //effectively making it so you cant grapple onto a platform/track within 2 tiles of an already attatched hook
                //more useful for multihooks, but still needed for single use hooks to make hooks not instantly grab to the tile you are on
                var hook = player.GetModPlayer<WheelchairPlayer>().curWheelGrapple;
                    
                if (hook!= null)
                {
                    Projectile projectile = hook.Projectile;

                    //hooks not latched onto something then skip
                    if ((projectile.ai[0] == 2f))
                    {
                        Point pt = projectile.Center.ToTileCoordinates();
                        Tile tileSafely = Framing.GetTileSafely(pt);

                        //if not a minecart track or a platform then skip
                        if ((tileSafely.TileType == 314 || TileID.Sets.Platforms[tileSafely.TileType]))
                        {
                            for (int j = -2; j <= 2; j++)
                            {
                                for (int k = -2; k <= 2; k++)
                                {
                                    Point point2 = new Point(pt.X + j, pt.Y + k);
                                    Tile tileSafely2 = Framing.GetTileSafely(point2);
                                    if (tileSafely2.TileType == 314 || TileID.Sets.Platforms[tileSafely2.TileType])
                                    {
                                        //gets a 5x5 area of minecart tracks/plats around original track/plat
                                        points.Add(point2);
                                    }
                                }
                            }
                        }
                    }
                }

                Vector2 vector4 = default(Vector2);
                //check 3x3 grid
                for (int l = topLeftCornerX; l < bottomRightCornerX; l++)
                {
                    for (int m = topLeftCornerY; m < bottomRightCornerY; m++)
                    {
                        //sets position incase of a future latch
                        vector4.X = l * 16;
                        vector4.Y = m * 16;
                        if (!(hitboxTopLeftCorner.X + 10f > vector4.X) || !(hitboxTopLeftCorner.X < vector4.X + 16f) || !(hitboxTopLeftCorner.Y + 10f > vector4.Y) || !(hitboxTopLeftCorner.Y < vector4.Y + 16f))
                        {
                            continue;
                        }
                        Tile tile = Main.tile[l, m];
                        if (!GrapplehookCanTileBeLatched(l, m) || points.Contains(new Point(l, m)) || (Projectile.type == 403 && tile.TileType != 314) || Main.player[Projectile.owner].IsBlacklistedForGrappling(new Point(l, m)))
                        {
                            continue;
                        }
                        if (Main.myPlayer != Projectile.owner)
                        {
                            continue;
                        }

                        //code responsible for dual hook effect, might want to use this tbh
                        if (Projectile.type == 73 || Projectile.type == 74)
                        {
                            for (int n = 0; n < 1000; n++)
                            {
                                if (n != Projectile.whoAmI && Main.projectile[n].active && Main.projectile[n].owner == Projectile.owner && Main.projectile[n].aiStyle == 7 && Main.projectile[n].ai[0] == 2f)
                                {
                                    Main.projectile[n].Kill();
                                }
                            }
                        }

                        //latching, particle and sound effects
                        WorldGen.KillTile(l, m, fail: true, effectOnly: true);
                        //playsound for grapple
                        //SoundEngine.PlaySound(0, vector4*16f);

                        Projectile.velocity.X = 0f;
                        Projectile.velocity.Y = 0f;
                        Projectile.ai[0] = 2f;
                        /* experiemntal, keeping the maxdist from the previous hook, seems better to just hold the down key when rehooking
                        if(player.GetModPlayer<WheelchairPlayer>().curWheelGrapple!= null)
                        {
                            maxDist = player.GetModPlayer<WheelchairPlayer>().curWheelGrapple.maxDist;
                        }
                        else
                        {
                            maxDist = MathHelper.Clamp(Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center), 0f, hookRange - 1f);
                        }
                        */
                        maxDist = MathHelper.Clamp(Vector2.Distance(Main.player[Projectile.owner].Center, Projectile.Center), 0f, hookRange - 1f);
                        DeleteOtherAttatchedHooks(player);

                        Projectile.position.X = l * 16 + 8 - Projectile.width / 2;
                        Projectile.position.Y = m * 16 + 8 - Projectile.height / 2;
                        Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(l, m);
                        if (tileVisualHitbox.HasValue)
                        {
                            Projectile.Center = tileVisualHitbox.Value.Center.ToVector2();
                        }
                        Projectile.damage = 0;
                        Projectile.netUpdate = true;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            //probably isnt needed as it syncs controlHook (probably)
                            //NetMessage.SendData(13, -1, -1, null, Projectile.owner);
                        }
                        break;
                    }
                    if (Projectile.ai[0] == 2f)
                    {
                        break;
                    }
                }
            }
            //grapple retracting
            else if (Projectile.ai[0] == 1f)
            {
                float rectractSpeed = 16f;
                GrappleRetreatSpeed(Main.player[Projectile.owner], ref rectractSpeed);
                if (distanceFromPlayer < 24f)
                {
                    Projectile.Kill();
                }
                distanceFromPlayer = rectractSpeed / distanceFromPlayer;
                xDistanceFromPlayer *= distanceFromPlayer;
                yDistanceFromPlayer *= distanceFromPlayer;
                Projectile.velocity.X = xDistanceFromPlayer;
                Projectile.velocity.Y = yDistanceFromPlayer;
            }
            //hooking a block
            else if (Projectile.ai[0] == 2f)
            {
                Point point3 = Projectile.Center.ToTileCoordinates();
                bool flag = true;
                if (GrapplehookCanTileBeLatched(point3.X, point3.Y))
                {
                    flag = false;
                }
                if (flag)
                {
                    Projectile.ai[0] = 1f;
                }
                else
                {
                    Main.player[Projectile.owner].GetModPlayer<WheelchairPlayer>().curWheelGrapple = this;
                }

                //make ai[1] track the number of active hooks?
                //ai[2] being the number of hooks currently latched on
            }
        }

        private bool GrapplehookCanTileBeLatched(int x, int y)
        {
            Tile theTile = Main.tile[x, y];
            bool vanilla = Main.tileSolid[theTile.TileType] | (theTile.TileType == 314);
            vanilla &= theTile.HasUnactuatedTile;
            return ProjectileLoader.GrappleCanLatchOnTo(Projectile, Main.player[Projectile.owner], x, y) ?? vanilla;
        }

        private bool noPulling = false;
        /// <summary>
        /// the maximum distance the hook can go, basically a soft hookrange, can be changed by using down and up while grappled... probably
        /// </summary>
        public float maxDist;

        //taken and modified slightly from terraria overhaul, makes them work like a swing point rather than a latch
        public void PlayerGrappleMovement(Player player, Projectile proj)
        {

            //I have no idea why this works, if I change anything it seems to break in weird ways
            var playerCenter = player.Center;
            var projCenter = proj.Center;


            var mountedCenter = player.MountedCenter;
            var mountedOffset = mountedCenter - projCenter;

            proj.rotation = (float)Math.Atan2(mountedOffset.Y, mountedOffset.X) - 1.57f;
            proj.velocity = Vector2.Zero;


            var dir = (playerCenter - projCenter).SafeNormalize(default);
            bool pull = !noPulling && player.controlHook;

            player.GoingDownWithGrapple = true;
            player.stairFall = true;

            // Prevent hooks from going farther than normal
            float ClampDistance(float distance)
                => MathHelper.Clamp(distance, 0f, hookRange - 1f);

            float dist = Vector2.Distance(playerCenter, projCenter);
            //maxDist = dist;
            //float dist = ClampDistance(Vector2.Distance(playerCenter, projCenter));
            bool down = player.controlDown && maxDist < hookRange;
            bool up = player.controlUp;

            const float PullSpeed = 12.5f;
            const float PullVelocity = 0.1f;
            const float RaiseSpeed = 5f;
            const float RaiseVelocity = 0.975f;
            const float LowerSpeed = 5f;
            const float LowerVelocity = 1f;

            if (pull || (up || down) && up != down)
            {
                maxDist = dist = ClampDistance(dist + (pull ? -PullSpeed : up ? -RaiseSpeed : LowerSpeed));

                player.velocity *= pull ? PullVelocity : up ? RaiseVelocity : LowerVelocity;
            }

            float nextDistance = Vector2.Distance(playerCenter + player.velocity, projCenter);
            float deltaDistance = nextDistance - dist;
            var vect = projCenter - playerCenter;
            float vectLength = vect.Length();
            float speedPlusGravity = deltaDistance + player.gravity;
            float maxSpeed = Math.Max(dist / 10f, 12f);

            player.velocity = Vector2.Clamp(player.velocity, Vector2.One * -maxSpeed, Vector2.One * maxSpeed);

            if ((player.controlLeft || player.controlRight) && (!player.controlRight || !player.controlLeft))
            {
                float accel = (player.controlLeft ? -6f : 6f) / 60f;

                player.velocity.X += accel;
                player.velocity = Vector2.Clamp(player.velocity, Vector2.One * -maxSpeed, Vector2.One * maxSpeed);
            }
            else
            {
                Vector2 desiredPosition = Projectile.Center;
                desiredPosition.Y += maxDist;
                desiredPosition = desiredPosition - player.Center;// - new Vector2(player.width/2f, 0f)
                //Main.NewText(player.velocity.X);
                //Main.NewText(desiredPosition);
                //this might need to be scaled with distance, but right now im not seeing any issues
                desiredPosition *= ((Math.Abs(desiredPosition.X) < 60f && Math.Abs(player.velocity.X) < .8f) ? (Math.Abs(desiredPosition.X) < 30f) ? .009f : .004f : .0009f);
                //desiredPosition *= (Math.Abs(player.velocity.X) > .8f) ? .0009f : (Math.Abs(desiredPosition.X) < 30f) ? .009f : (Math.Abs(desiredPosition.X) < 60f) ? .004f : .0009f;
                player.velocity.X += desiredPosition.X;
            }

            float tempVal;

            if (vectLength > speedPlusGravity)
            {
                tempVal = speedPlusGravity / vectLength;
            }
            else
            {
                tempVal = 1f;
            }

            vect *= tempVal;

            if (dist >= maxDist)
            {
                
                player.velocity += vect;
                
                //player.maxRunSpeed = 15f;
                //player.runAcceleration *= 3f;
                
            }
            else
            {
                player.runAcceleration = 0f;
                player.moveSpeed = 0f;
            }
        }

        //taken and modified from terraria overhaul
        public static void PlayerJumpOffGrapplingHook(Player player)
        {
            if (player.controlJump && player.releaseJump)
            {
                player.velocity.Y = Math.Min(player.velocity.Y, -Player.jumpSpeed);
                player.jump = 0;

                player.releaseJump = false;

                player.RefreshMovementAbilities();
                player.RemoveAllGrapplingHooks();
            }
        }

        public override bool PreDrawExtras()
        {
            Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;
            float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceToPlayer = directionToPlayer.Length();

            while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer))
            {
                directionToPlayer /= distanceToPlayer; // get unit vector
                directionToPlayer *= chainTexture.Height(); // multiply by chain link length

                center += directionToPlayer; // update draw position
                directionToPlayer = playerCenter - center; // update distance
                distanceToPlayer = directionToPlayer.Length();

                Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

                // Draw chain
                Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
                    chainTexture.Value.Bounds, drawColor, chainRotation,
                    chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
            }

            // Stop vanilla from drawing the default chain.
            return false;
        }

        public override float GrappleRange()
        {
            return hookRange;
        }

        public override void GrappleRetreatSpeed(Player player, ref float speed)
        {
            speed = 16f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(maxDist);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            maxDist = reader.ReadSingle();
        }
    }
}