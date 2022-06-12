using System;

namespace CopyRouteMod
{
    public class CopyRouteModEntryPoint : IMatiMod
    {
        public CopyRouteModEntryPoint()
        {
            Console.WriteLine($"{nameof(CopyRouteModEntryPoint)} constructed");
        }

        public void Load()
        {
            Console.WriteLine($"{nameof(CopyRouteModEntryPoint)} loaded");

            Sweet_Transit.Main.main.Components.Add(new CopyRouteGameComponent());
        }
    }
}