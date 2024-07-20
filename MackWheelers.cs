using System.IO;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MackWheelers.Content.Items.WheelchairAccessories;
using MackWheelers.Content.Players;

namespace MackWheelers
{
    public class MackWheelers : Mod
	{
        public override void Load()
        {
            //no idea if this is the right place to do this lmao
            WheelchairAccMaxValues.ItemSlotsMaxs.Add(WheelchairAccessoryTypeEnum.Wheels, 3);
            WheelchairAccMaxValues.ItemSlotsMaxs.Add(WheelchairAccessoryTypeEnum.Box, 3);
            WheelchairAccMaxValues.ItemSlotsMaxs.Add(WheelchairAccessoryTypeEnum.Sidearm, 2);
            WheelchairAccMaxValues.ItemSlotsMaxs.Add(WheelchairAccessoryTypeEnum.Shoulder, 2);
            WheelchairAccMaxValues.ItemSlotsMaxs.Add(WheelchairAccessoryTypeEnum.Hull, 2);
            WheelchairAccMaxValues.ItemSlotsMaxs.Add(WheelchairAccessoryTypeEnum.Undercarriage, 2);
            WheelchairAccMaxValues.ItemSlotsMaxs.Add(WheelchairAccessoryTypeEnum.Back, 2);
            WheelchairAccMaxValues.ItemSlotsMaxs.Add(WheelchairAccessoryTypeEnum.Engine, 1);
            base.Load();
        }


        internal enum MessageType : byte
        {
            DefaultPlayerSync,
            PlayerStartPushing,
            PlayerStopPushing,
            PlayerStopPushingThrow,
            ForceStopPushing,
            WheelchairMinecartShift//,
            //SendWheelchairAccessorys
        }

        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            MessageType msgType = (MessageType)reader.ReadByte();


            switch (msgType)
            {
                case MessageType.DefaultPlayerSync:
                    {
                        byte playerNumber = reader.ReadByte();
                        WheelchairPlayer wheeliePlayer = Main.player[playerNumber].GetModPlayer<WheelchairPlayer>();
                        wheeliePlayer.ReceiveDefaultSyncPacket(reader, whoAmI);

                        //Main.NewText("got thing");

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            wheeliePlayer.SyncPlayer(-1, whoAmI, false);
                        }
                    }

                    break;
                case MessageType.PlayerStartPushing:
                    {
                        byte playerNumber = reader.ReadByte();
                        WheelchairPlayer wheeliePlayer = Main.player[playerNumber].GetModPlayer<WheelchairPlayer>();
                        wheeliePlayer.ReceiveStartPushingPacket(reader, whoAmI);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            wheeliePlayer.SendStartPushingPacket(-1, whoAmI);
                        }
                    }
                    break;
                case MessageType.PlayerStopPushing:
                    {
                        byte playerNumber = reader.ReadByte();
                        WheelchairPlayer wheeliePlayer = Main.player[playerNumber].GetModPlayer<WheelchairPlayer>();
                        wheeliePlayer.ReceiveStopPushingPacket(reader);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            wheeliePlayer.SendStopPushingPacket(-1, whoAmI);
                        }
                    }
                    break;
                case MessageType.PlayerStopPushingThrow:
                    {
                        byte playerNumber = reader.ReadByte();
                        WheelchairPlayer wheeliePlayer = Main.player[playerNumber].GetModPlayer<WheelchairPlayer>();
                        wheeliePlayer.ReceiveStopPushingAndThrowPacket(reader);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            wheeliePlayer.SendStopPushingAndThrowPacket(-1, whoAmI);
                        }
                    }
                    break;
                case MessageType.ForceStopPushing:
                    {
                        byte playerNumber = reader.ReadByte();
                        WheelchairPlayer wheeliePlayer = Main.player[playerNumber].GetModPlayer<WheelchairPlayer>();
                        wheeliePlayer.ReceiveForceStopPushingPacket(reader);

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            wheeliePlayer.SendForceStopPushingPacket(-1, whoAmI);
                        }
                    }
                    break;
                case MessageType.WheelchairMinecartShift:
                    /*
                    if (Main.npc[reader.ReadByte()].ModNPC is ExamplePerson person && person.NPC.active)
                    {
                        person.StatueTeleport();
                    }*/

                    break;
                    /*
                case MessageType.SendWheelchairAccessorys:
                    {
                        byte playerNumber = reader.ReadByte();
                        WheelchairPlayer wheeliePlayer = Main.player[playerNumber].GetModPlayer<WheelchairPlayer>();

                        if (Main.netMode == NetmodeID.Server)
                        {
                            // Forward the changes to the other clients
                            wheeliePlayer.SendWheelAccsPacket(-1, whoAmI);
                        }
                    }
                    break;*/
                default:
                    Logger.WarnFormat("MackWheelers: Unknown Message type: {0}", msgType);
                    break;
            }
        }
    }
}