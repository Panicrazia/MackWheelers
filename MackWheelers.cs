using System.IO;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace MackWheelers
{
	public class MackWheelers : Mod
	{

        internal enum MessageType : byte
        {
            DefaultPlayerSync,
            PlayerStartPushing,
            PlayerStopPushing,
            PlayerStopPushingThrow,
            ForceStopPushing,
            WheelchairMinecartShift
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
                default:
                    Logger.WarnFormat("MackWheelers: Unknown Message type: {0}", msgType);
                    break;
            }
        }
    }
}