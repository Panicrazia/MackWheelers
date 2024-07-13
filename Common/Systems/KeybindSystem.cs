using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace MackWheelers.Common.Systems
{
    internal class KeybindSystem : ModSystem
    {
        //public static ModKeybind WheelchairGrappleKeybind { get; private set; }

        public override void Load()
        {
            //WheelchairGrappleKeybind = KeybindLoader.RegisterKeybind(Mod, "WheelchairGrapple", "P");
        }


        // Please see ExampleMod.cs' Unload() method for a detailed explanation of the unloading process.
        public override void Unload()
        {
            // Not required if your AssemblyLoadContext is unloading properly, but nulling out static fields can help you figure out what's keeping it loaded.

            //WheelchairGrappleKeybind = null;
        }
    }
}
