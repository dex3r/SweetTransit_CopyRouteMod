using System;
using System.IO;
using MatiModLoader;
using ModuleInitializerInjector;

namespace MatiModLoaderPatcher
{
    public class Patcher
    {
        public static void PatchMatiModLoader(string matiModLoaderDllPath)
        {
            if (File.Exists(matiModLoaderDllPath) == false)
            {
                throw new Exception($"Could not find mati mod loader dll path under '{matiModLoaderDllPath}'");
            }

            new Injector().Inject(matiModLoaderDllPath, nameof(Startup), nameof(Startup.Initialize));
        }
    }
}