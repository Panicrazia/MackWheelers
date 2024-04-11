using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameInput;
using Terraria.UI;
using Terraria;
using Terraria.GameContent;
using Microsoft.Xna.Framework;
using MackWheelers.Content.Items;
using MackWheelers.Content.Items.Mounts;
using Terraria.ModLoader;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace MackWheelers.Common.UI
{
    // This class wraps the vanilla ItemSlot class into a UIElement. The ItemSlot class was made before the UI system was made, so it can't be used normally with UIState. 
    // By wrapping the vanilla ItemSlot class, we can easily use ItemSlot.
    // ItemSlot isn't very modder friendly and operates based on a "Context" number that dictates how the slot behaves when left, right, or shift clicked and the background used when drawn. 
    // If you want more control, you might need to write your own UIElement.
    // I've added basic functionality for validating the item attempting to be placed in the slot via the validItem Func. 
    // See ExamplePersonUI for usage and use the Awesomify chat option of Example Person to see in action.
    internal class WheelchairAccessoryItemSlotWrapper : UIElement
    {
        internal Item Item;
        private readonly int _context;
        private readonly float _scale;
        private WheelchairAccessoryTypeEnum accType;

        public WheelchairAccessoryItemSlotWrapper(WheelchairAccessoryTypeEnum accType, bool left, float scale = UIConstants.ITEMSLOTMULTIPLIER, int context = ItemSlot.Context.BankItem)
        {
            this.accType = accType;
            _context = context;
            _scale = scale;
            if (!left)
            {
                MarginLeft = 1f;    //shutup its needed
            }
            MarginTop = 1f;
            Item = new Item();
            Item.SetDefaults(0);
            //Item.SetDefaults(324);//illegal gun parts for test

            Width.Set(TextureAssets.InventoryBack9.Width() * scale, 0f);
            Height.Set(TextureAssets.InventoryBack9.Height() * scale, 0f);
        }

        public Item GetItem() { return Item; }

        public void SetItem(Item item)
        {
            Item = item;
            //Recalculate();
        }

        public void SetItemToAir()
        {
            Item = new Item();
            Item.SetDefaults(0);
            Recalculate();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            if (ContainsPoint(Main.MouseScreen) && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                if (VerifyItem(Main.mouseItem))
                {
                    // Handle handles all the click and hover actions based on the context.
                    ItemSlot.Handle(ref Item, _context);


                    if ((Main.mouseLeftRelease && Main.mouseLeft))
                    {
                        if (accType == WheelchairAccessoryTypeEnum.Wheelchair)
                        {
                            UIElement parentParent = Parent.Parent;
                            if (parentParent is WheelchairAccessoryUI)
                            {
                                //Main.NewText("ladies and gentlemen, weve gottem");
                                (parentParent as WheelchairAccessoryUI).SetWheelchairType((Item.ModItem as BaseWheelchairItem).wheelchairType);
                                (parentParent as WheelchairAccessoryUI).PopulateSections((Item.ModItem as BaseWheelchairItem).wheelchairAccessoryList);
                            }
                        }
                        else
                        {
                            UIElement parentParentParent = Parent.Parent.Parent;
                            (parentParentParent as WheelchairAccessoryUI).AddItem(Item);
                        }
                    }
                }
                else if (Main.mouseItem.IsAir && Item != null)
                {
                    if ((Main.mouseLeftRelease && Main.mouseLeft))
                    {
                        UIElement parentParent = Parent.Parent;
                        if (parentParent is WheelchairAccessoryUI)
                        {
                            (parentParent as WheelchairAccessoryUI).SetWheelchairType(WheelchairType.None);
                        }
                        else
                        {
                            UIElement parentParentParent = Parent.Parent.Parent;
                            (parentParentParent as WheelchairAccessoryUI).RemoveItem(Item);
                        }
                    }

                    //move the things depending on this to here so we can actually remove properly
                    ItemSlot.Handle(ref Item, _context);
                }
            }
            // Draw draws the slot itself and Item. Depending on context, the color will change, as will drawing other things like stack counts.
            ItemSlot.Draw(spriteBatch, ref Item, _context, rectangle.TopLeft());
            Main.inventoryScale = oldScale;
        }

        public bool VerifyItem(Item item)
        {
            if (item.ModItem is WheelchairWheelAcc)
            {
                if (accType == (item.ModItem as WheelchairWheelAcc).GetAccType())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (item.ModItem is BaseWheelchairItem)
            {
                if (accType == (item.ModItem as BaseWheelchairItem).enumType)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
