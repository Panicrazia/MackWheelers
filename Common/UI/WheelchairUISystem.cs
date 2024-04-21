using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace MackWheelers.Common.UI
{
    [Autoload(Side = ModSide.Client)] // This attribute makes this class only load on a particular side. Naturally this makes sense here since UI should only be a thing clientside. Be wary though that accessing this class serverside will error
    public class WheelchairUISystem : ModSystem
    {
        private UserInterface wheelchairWorkshopUIinterface;
        internal WheelchairAccessoryUI wheelchairUI;

        // These two methods will set the state of our custom UI, causing it to show or hide
        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the UI was open beforehand</returns>
        public bool ToggleUI()
        {
            if(wheelchairWorkshopUIinterface?.CurrentState != null)
            {
                HideMyUI();
                return true;
            }
            else
            {
                ShowMyUI();
                return false;
            }

        }

        public void ShowMyUI()
        {
            wheelchairWorkshopUIinterface?.SetState(wheelchairUI);
        }

        public void HideMyUI()
        {
            wheelchairWorkshopUIinterface?.SetState(null);
        }

        public void wheelchairAccChange()
        {
            if (wheelchairWorkshopUIinterface?.CurrentState != null && wheelchairWorkshopUIinterface?.CurrentState is WheelchairAccessoryUI)
            {
                ((WheelchairAccessoryUI)wheelchairWorkshopUIinterface?.CurrentState).UpdateWheelchair();
            }
            
        }

        public override void Load()
        {
            if (!Main.dedServ)
            {
                // Create custom interface which can swap between different UIStates
                wheelchairWorkshopUIinterface = new UserInterface();
                // Creating custom UIState
                wheelchairUI = new WheelchairAccessoryUI();

                // Activate calls Initialize() on the UIState if not initialized, then calls OnActivate and then calls Activate on every child element
                wheelchairUI.Activate();
            }
        }

        private GameTime _lastUpdateUiGameTime;

        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (wheelchairWorkshopUIinterface?.CurrentState != null)
            {
                wheelchairWorkshopUIinterface.Update(gameTime);
            }
        }

        // Adding a custom layer to the vanilla layer list that will call .Draw on your interface if it has a state
        // Setting the InterfaceScaleType to UI for appropriate UI scaling
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Mackwheelers: WheelchairWorkshop",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && wheelchairWorkshopUIinterface?.CurrentState != null)
                        {
                            wheelchairWorkshopUIinterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

    }
}
