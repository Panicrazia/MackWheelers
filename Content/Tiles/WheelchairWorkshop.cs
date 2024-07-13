using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ID;
using Terraria.Localization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Microsoft.Xna.Framework;
using MackWheelers.Common.UI;

namespace MackWheelers.Content.Tiles
{
    internal class WheelchairWorkshop : ModTile
	{
        //TODO: make it so when you step away the ui closes and you get the wheelchair back in your inventory/on the ground
		public override void SetStaticDefaults()
        {
            // Properties
            Main.tileSolidTop[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = false;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.HasOutlines[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileID.Sets.BasicDresser[Type] = false;
            TileID.Sets.AvoidedByNPCs[Type] = true;
            TileID.Sets.InteractibleByNPCs[Type] = true;
            TileID.Sets.IsAContainer[Type] = true;

            //mabs make this a crafting station for wheelchair parts?
            //AdjTiles = new int[] { TileID.Dressers };
            DustType = DustID.WoodFurniture;

            // Names
            AddMapEntry(new Color(200, 200, 200), CreateMapEntryName());

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.HookCheckIfCanPlace = new PlacementHook(Chest.FindEmptyChest, -1, 0, true);
            TileObjectData.newTile.HookPostPlaceMyPlayer = new PlacementHook(Chest.AfterPlacement_Hook, -1, 0, false);
            TileObjectData.newTile.AnchorInvalidTiles = new int[] {
                    TileID.MagicalIceBlock,
                    TileID.Boulder,
                    TileID.BouncyBoulder,
                    TileID.LifeCrystalBoulder,
                    TileID.RollingCactus
                };
            TileObjectData.newTile.LavaDeath = false;
            TileObjectData.addTile(Type);
        }

        public override LocalizedText DefaultContainerName(int frameX, int frameY)
        {
            return CreateMapEntryName();
        }

        public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
        {
            return true;
        }

        /*
        public override void ModifySmartInteractCoords(ref int width, ref int height, ref int frameWidth, ref int frameHeight, ref int extraY)
        {
            width = 3;
            height = 2;
            extraY = 0;
        }*/

        public override bool RightClick(int i, int j)
        {
            Player player = Main.LocalPlayer;
            /*
            int left = Main.tile[i, j].TileFrameX / 18;
            left %= 3;
            left = i - left;
            int top = j - Main.tile[i, j].TileFrameY / 18;
            */


            //Main.interactedDresserTopLeftX = left;
            //Main.interactedDresserTopLeftY = top;

            //Main.NewText("tried to open");
            if (ModContent.GetInstance<WheelchairUISystem>().ToggleUI())
            {
                Main.playerInventory = false;
                player.chest = -1;
                Recipe.FindRecipes();
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = "";
            }
            else
            {
                Main.playerInventory = true;
                player.chest = -1;
                Recipe.FindRecipes();
                player.SetTalkNPC(-1);
                Main.npcChatCornerItem = 0;
                Main.npcChatText = "";
                ModContent.GetInstance<WheelchairUISystem>().wheelchairAccChange();
            }


            return true;
        }
    }
}
