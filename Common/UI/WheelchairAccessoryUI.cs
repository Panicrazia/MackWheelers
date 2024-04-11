using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using MackWheelers.Content.Items;
using System.Collections;
using Terraria.GameContent.Achievements;
using Humanizer;
using Terraria.ModLoader.UI;
using Microsoft.Xna.Framework.Input;
using Terraria.GameContent.Events;
using MackWheelers.Content.Items.Mounts;

namespace MackWheelers.Common.UI
{
    public enum WheelchairType
    {
        None,
        Wooden,
        Electric,
        Reinforced,
        Advanced
    }

    public static class UIConstants
    {
        //blame base terraira for these stupid constants
        public const float ITEMSLOTREALSIZE = 52f;
        public const float ITEMSLOTMULTIPLIER = .6f;
        public const float ITEMSLOTSIZE = ITEMSLOTREALSIZE * ITEMSLOTMULTIPLIER; 
        /*
         * on my computer when opening a ui like a chest Main.inventoryScale is .6, item slots are ultra hardcoded, 
         * if this turns out to be different on other peoples machines ill add a config for other inventory scales
         * 
         * doing this in an actually nice way while still using the vanilla terraria itemslots is near if not impossible
         * 
         */
        public const float ITEMSLOTSPACEHORIZONTAL = ITEMSLOTSIZE / 8f;
        public const float ITEMSLOTSPACEVERTICAL = ITEMSLOTSIZE / 4f;
        public const float WHEELCHAIRWORKSHOPPADDING = ITEMSLOTSIZE / 4f;
        public const float WHEELCHAIRWORKSHOPWIDTH = (ITEMSLOTSIZE * 7f) + (ITEMSLOTSPACEHORIZONTAL * 6f) + (WHEELCHAIRWORKSHOPPADDING * 2f);
        public const float WHEELCHAIRWORKSHOPHEIGHT = (ITEMSLOTSIZE * 4f) + (ITEMSLOTSPACEVERTICAL * 3f) + (WHEELCHAIRWORKSHOPPADDING * 2f);
    }

    internal class WheelchairAccessoryUI : UIState
    {
        public DraggableUIPanel wheelchairAccessoryWorkshop;
        public WheelchairType wheelchairType = WheelchairType.None;

        public Dictionary<WheelchairAccessoryTypeEnum, bool> specialWheelchairUpgrades = new Dictionary<WheelchairAccessoryTypeEnum, bool>();
        public Dictionary<WheelchairAccessoryTypeEnum, UIItemSlotSection> ItemSlotSections = new Dictionary<WheelchairAccessoryTypeEnum, UIItemSlotSection>();
        public Dictionary<WheelchairAccessoryTypeEnum, List<WheelchairAccessoryItemSlotWrapper>> ItemSlots = new Dictionary<WheelchairAccessoryTypeEnum, List<WheelchairAccessoryItemSlotWrapper>>();
        
        public WheelchairAccessoryItemSlotWrapper WheelchairSlot;

        
        public override void OnInitialize()
        {
            wheelchairAccessoryWorkshop = new DraggableUIPanel();
            wheelchairAccessoryWorkshop.SetPadding(0); //lol. lmao.

            SetSelfRectangle(wheelchairAccessoryWorkshop, Main.screenWidth * .5f, Main.screenHeight * .5f, width: UIConstants.WHEELCHAIRWORKSHOPWIDTH, height: UIConstants.WHEELCHAIRWORKSHOPHEIGHT);
            wheelchairAccessoryWorkshop.BorderColor = Color.White;
            wheelchairAccessoryWorkshop.BackgroundColor = new Color(73, 94, 171);


            //wheelchair itemslot
            WheelchairSlot = new WheelchairAccessoryItemSlotWrapper(WheelchairAccessoryTypeEnum.Wheelchair, true, 1f);
            SetWheelchairItemSlot(WheelchairSlot);
            wheelchairAccessoryWorkshop.Append(WheelchairSlot);

            /*
            //close button
            Asset<Texture2D> buttonDeleteTexture = ModContent.Request<Texture2D>("Terraria/Images/UI/ButtonDelete");
            UIImageButton closeButton = new UIImageButton(buttonDeleteTexture);
            SetRectangleRight(closeButton, left: -40f, top: 10f, width: 22f, height: 22f);
            closeButton.OnLeftClick += new MouseEvent(CloseButtonClicked);
            wheelchairAccessoryWorkshop.Append(closeButton);

            //WE DONT NEED A CLOSE BUTTON JUST MAKE IT CLOSE WHEN THEY GET FAR  
            //just drop any items that arent already installed
                //this is assuming I have a confirm/install button to 'register' accessories into the wheelchair

            */

            SetUpSections();

            CreateSections();

            CreateSections2();

            //WheelchairSlot = new WheelchairAccessoryItemSlotWrapper(WheelchairAccessoryTypeEnum.Wheels);
            //SetSelfRectangle(WheelchairSlot, 15f, 20f, UIConstants.ITEMSLOTSIZE, UIConstants.ITEMSLOTSIZE);

            //See if changing the width here is possible


            Append(wheelchairAccessoryWorkshop);
        }

        public void SetWheelchairType(WheelchairType newType)
        {
            Main.NewText("old wheelchairType: " + wheelchairType);
            wheelchairType = newType;
            Main.NewText("new wheelchairType: " + wheelchairType);
            ReDoItemSlots();
        }

        public void ReDoItemSlots()
        {
            WipeSections();
            CreateSections();
            CreateSections2();
        }

        public void WipeSections()
        {
            //this should remove all slots that arent the base wheelchair slot
            foreach (KeyValuePair<WheelchairAccessoryTypeEnum, UIItemSlotSection> entry in ItemSlotSections)
            {
                entry.Value.RemoveAllItemSlots();
            }
            foreach (KeyValuePair<WheelchairAccessoryTypeEnum, List<WheelchairAccessoryItemSlotWrapper>> entry in ItemSlots)
            {
                entry.Value.Clear();
            }
        }

        public void CreateSections()
        {
            switch (wheelchairType)
            {
                case (WheelchairType.Advanced):
                    GetNewSlot(WheelchairAccessoryTypeEnum.Box);
                    GetNewSlot(WheelchairAccessoryTypeEnum.Undercarriage);
                    GetNewSlot(WheelchairAccessoryTypeEnum.Shoulder);
                    goto case WheelchairType.Reinforced;
                case (WheelchairType.Reinforced):
                    GetNewSlot(WheelchairAccessoryTypeEnum.Sidearm);
                    GetNewSlot(WheelchairAccessoryTypeEnum.Back);
                    goto case WheelchairType.Electric;
                case (WheelchairType.Electric):
                    GetNewSlot(WheelchairAccessoryTypeEnum.Wheels);
                    GetNewSlot(WheelchairAccessoryTypeEnum.Box);
                    GetNewSlot(WheelchairAccessoryTypeEnum.Hull);
                    GetNewSlot(WheelchairAccessoryTypeEnum.Undercarriage);
                    GetNewSlot(WheelchairAccessoryTypeEnum.Engine);
                    goto case WheelchairType.Wooden;
                case (WheelchairType.Wooden):
                    GetNewSlot(WheelchairAccessoryTypeEnum.Wheels);
                    GetNewSlot(WheelchairAccessoryTypeEnum.Shoulder);
                    break;
                default:
                    break;
            }
        }
        public void CreateSections2()
        {
            foreach (KeyValuePair<WheelchairAccessoryTypeEnum, bool> entry in specialWheelchairUpgrades)
            {
                //TODO: do
            }
        }

        public void PopulateSections(Dictionary<WheelchairAccessoryTypeEnum, List<Item>> itemDict)
        {
            /*
            foreach (KeyValuePair<WheelchairAccessoryTypeEnum, List<WheelchairAccessoryItemSlotWrapper>> entry in ItemSlots)
            {
                foreach (var item in entry.Value.Select((value, i) => new { i, value }))
                {
                    //assuming this goes from low index to high then this should be fine
                    var value = item.value;
                    var index = item.i;
                }
                //shallow copy should be fine right???
                ItemSlots = itemDict
            }
            */

            //i dunno if this is a shallow copy or not but if it works idk
            foreach (KeyValuePair<WheelchairAccessoryTypeEnum, List<Item>> entry in itemDict)
            {
                foreach (var item in entry.Value.Select((value, i) => new { i, value }))
                {
                    var value = item.value;
                    var index = item.i;

                    ItemSlots[entry.Key][index].SetItem(value);
                }
            }
        }

        private void SetUpSections()
        {
            ItemSlotSections.Add(WheelchairAccessoryTypeEnum.Wheels, new UIItemSlotSection(wheelchairAccessoryWorkshop, true, 0));
            ItemSlotSections.Add(WheelchairAccessoryTypeEnum.Back, new UIItemSlotSection(wheelchairAccessoryWorkshop, true, 1));
            ItemSlotSections.Add(WheelchairAccessoryTypeEnum.Sidearm, new UIItemSlotSection(wheelchairAccessoryWorkshop, true, 2));
            ItemSlotSections.Add(WheelchairAccessoryTypeEnum.Engine, new UIItemSlotSection(wheelchairAccessoryWorkshop, true, 3));
            ItemSlotSections.Add(WheelchairAccessoryTypeEnum.Box, new UIItemSlotSection(wheelchairAccessoryWorkshop, false, 0));
            ItemSlotSections.Add(WheelchairAccessoryTypeEnum.Shoulder, new UIItemSlotSection(wheelchairAccessoryWorkshop, false, 1));
            ItemSlotSections.Add(WheelchairAccessoryTypeEnum.Hull, new UIItemSlotSection(wheelchairAccessoryWorkshop, false, 2));
            ItemSlotSections.Add(WheelchairAccessoryTypeEnum.Undercarriage, new UIItemSlotSection(wheelchairAccessoryWorkshop, false, 3));

            ItemSlots.Add(WheelchairAccessoryTypeEnum.Wheels, new List<WheelchairAccessoryItemSlotWrapper>());
            ItemSlots.Add(WheelchairAccessoryTypeEnum.Box, new List<WheelchairAccessoryItemSlotWrapper>());
            ItemSlots.Add(WheelchairAccessoryTypeEnum.Sidearm, new List<WheelchairAccessoryItemSlotWrapper>());
            ItemSlots.Add(WheelchairAccessoryTypeEnum.Shoulder, new List<WheelchairAccessoryItemSlotWrapper>());
            ItemSlots.Add(WheelchairAccessoryTypeEnum.Hull, new List<WheelchairAccessoryItemSlotWrapper>());
            ItemSlots.Add(WheelchairAccessoryTypeEnum.Undercarriage, new List<WheelchairAccessoryItemSlotWrapper>());
            ItemSlots.Add(WheelchairAccessoryTypeEnum.Back, new List<WheelchairAccessoryItemSlotWrapper>());
            ItemSlots.Add(WheelchairAccessoryTypeEnum.Engine, new List<WheelchairAccessoryItemSlotWrapper>());

            specialWheelchairUpgrades.Add(WheelchairAccessoryTypeEnum.Wheels, false);
        }

        private WheelchairAccessoryItemSlotWrapper GetNewSlot(WheelchairAccessoryTypeEnum type)
        {
            List<WheelchairAccessoryItemSlotWrapper> slots = ItemSlots[type];
            if (slots != null && slots.Count <= WheelchairAccMaxValues.ItemSlotsMaxs[type])
            {
                bool left = ItemSlotSections[type].leftOriented;
                WheelchairAccessoryItemSlotWrapper newSlot = new WheelchairAccessoryItemSlotWrapper(type, left);
                ItemSlotSections[type].AddItemSlot(newSlot, slots.Count);
                slots.Add(newSlot);
                return newSlot;
            }
            else
            {
                Main.NewText("tried to add a slot but there was no space");
                Main.NewText("yeah i dunno bruv go fuk urself lmao");
                return null;
            }
        }

        protected void SetWheelchairItemSlot(UIElement uiElement)
        {
            uiElement.Left.Set((UIConstants.WHEELCHAIRWORKSHOPWIDTH *.5f) - (UIConstants.ITEMSLOTREALSIZE * .5f), 0f);
            uiElement.Top.Set((UIConstants.WHEELCHAIRWORKSHOPHEIGHT *.5f) - (UIConstants.ITEMSLOTREALSIZE * .5f), 0f);
            uiElement.Width.Set(UIConstants.ITEMSLOTREALSIZE, 0f);
            uiElement.Height.Set(UIConstants.ITEMSLOTREALSIZE, 0f);
        }

        protected void SetSelfRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        public void CloseButtonClicked(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
            ModContent.GetInstance<WheelchairUISystem>().HideMyUI();
        }

        /// <summary>
        /// adds the item to the wheelchair item
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(Item item)
        {
            BaseWheelchairItem wheelchair = WheelchairSlot.GetItem().ModItem as BaseWheelchairItem;

            wheelchair.AddItem(item);
        }

        /// <summary>
        /// removes the item from the wheelchair item
        /// </summary>
        public void RemoveItem(Item item)
        {
            BaseWheelchairItem wheelchair = WheelchairSlot.GetItem().ModItem as BaseWheelchairItem;

            wheelchair.RemoveItem(item);
        }
    }

    internal class UIItemSlotSection : UIElement
    {
        public bool leftOriented;
        public UIItemSlotSection(UIElement uiElement, bool left, int count) : base()
        {
            if(left)
            {
                AltSetRectangle(this, UIConstants.WHEELCHAIRWORKSHOPPADDING, UIConstants.WHEELCHAIRWORKSHOPPADDING + (count * (UIConstants.ITEMSLOTSIZE + UIConstants.ITEMSLOTSPACEVERTICAL)), 100f, 100f);
            }
            else
            {
                AltSetRectangleRight(this, UIConstants.WHEELCHAIRWORKSHOPPADDING, UIConstants.WHEELCHAIRWORKSHOPPADDING + (count * (UIConstants.ITEMSLOTSIZE + UIConstants.ITEMSLOTSPACEVERTICAL)), 100f, 100f);
            }
            leftOriented = left;
            uiElement.Append(this);
        }
        /*
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         *  WE WANT MARGIN LEFT TO BE 1 ON RIGHT SIDE ITEMSLOTS
         * 
         *  AND WE WANT MARGIN LEFT TO BE 0 ON LEFT SIDE ITEMSLOTS
         * 
         *  THIS MAKES THEM SYMMETRICAL
         * 
         *  FUCK
         * 
         * 
         * 
         * 
         * 
         * 
         * 
         */
        public void AddItemSlot(WheelchairAccessoryItemSlotWrapper thing, int count)
        {
            if (leftOriented)
            {
                SetRectangle(thing, count * (UIConstants.ITEMSLOTSIZE + UIConstants.ITEMSLOTSPACEHORIZONTAL), 0f, UIConstants.ITEMSLOTSIZE, UIConstants.ITEMSLOTSIZE);
                thing.MarginLeft = 0f;
            }
            else
            {
                SetRectangleRight(thing, count * (UIConstants.ITEMSLOTSIZE + UIConstants.ITEMSLOTSPACEHORIZONTAL), 0f, UIConstants.ITEMSLOTSIZE, UIConstants.ITEMSLOTSIZE);
                thing.MarginLeft = 1f;
            }
            this.Append(thing);
        }

        public void RemoveAllItemSlots()
        {
            this.RemoveAllChildren();
        }

        public List<Item> GetItemsInside()
        {
            //TODO: have these be saved into the wheelchair
            List<Item> items = new List<Item>();
            return items;
        }

        protected void SetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }
        private void SetRectangleRight(UIElement uiElement, float left, float top, float width, float height)
        {
            float actualLeft = 100f - left - width;
            uiElement.Left.Set(actualLeft, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        public void AltSetRectangle(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(left, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }
        public void AltSetRectangleRight(UIElement uiElement, float left, float top, float width, float height)
        {
            float actualLeft = UIConstants.WHEELCHAIRWORKSHOPWIDTH - left - width;
            uiElement.Left.Set(actualLeft, 0f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }

        /*
        public void AltSetRectangleRightOld(UIElement uiElement, float left, float top, float width, float height)
        {
            uiElement.Left.Set(-left, 1f);
            uiElement.Top.Set(top, 0f);
            uiElement.Width.Set(width, 0f);
            uiElement.Height.Set(height, 0f);
        }
        */
        /* just gonna hope that the default drawself will be enough
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle innerDimensions = GetInnerDimensions();
            // Getting top left position of this UIElement
            float shopx = innerDimensions.X;
            float shopy = innerDimensions.Y;
        
            // Drawing first line of coins (current collected coins)
            // CoinsSplit converts the number of copper coins into an array of all types of coins
            DrawCoins(spriteBatch, shopx, shopy, Utils.CoinsSplit(collectedCoins));

            // Drawing second line of coins (coins per minute) and text "CPM"
            DrawCoins(spriteBatch, shopx, shopy, Utils.CoinsSplit(GetCoinsPerMinute()), 0, 25);
            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, "CPM", shopx + (float)(24 * 4), shopy + 25f, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
        }

        private void DrawCoins(SpriteBatch spriteBatch, float shopx, float shopy, int[] coinsArray, int xOffset = 0, int yOffset = 0)
        {
            for (int j = 0; j < 4; j++)
            {
                spriteBatch.Draw(coinsTextures[j], new Vector2(shopx + 11f + 24 * j + xOffset, shopy + yOffset), null, Color.White, 0f, coinsTextures[j].Size() / 2f, 1f, SpriteEffects.None, 0f);
                Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.ItemStack.Value, coinsArray[3 - j].ToString(), shopx + 24 * j + xOffset, shopy + yOffset, Color.White, Color.Black, new Vector2(0.3f), 0.75f);
            }
        }
        */
    }
}
