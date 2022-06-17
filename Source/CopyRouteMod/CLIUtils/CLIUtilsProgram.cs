using System;
using STDepsPatcher;
using STModLoaderInjection;

namespace CLIUtils
{
    class CLIUtilsProgram
    {
        static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Expected more than 0 arguments");
                return -2;
            }

            bool IsCmd(string command)
            {
                return args[0].Equals(command, StringComparison.InvariantCultureIgnoreCase);
            }

            if (IsCmd("Publish"))
            {
                Publisher.Publish();
            }
            else if (IsCmd("CopyToGame"))
            {
                Publisher.Publish();
                GameDirectoryManager.CopyModToGame();
            }
            else if (IsCmd("CopyToGame&Launch"))
            {
                Publisher.Publish();
                GameDirectoryManager.CopyModToGame();
                GameDirectoryManager.LaunchGame();
            }
            else if (IsCmd("PatchDeps"))
            {
                PatchDeps();
            }
            else if (IsCmd("InjectModLoaderIntoGameDll"))
            {
                InjectModLoader();
            }
            else if (IsCmd("InstallModLoader"))
            {
                PatchDeps();
                InjectModLoader();
            }
            else
            {
                Console.WriteLine($"Unknown argument {args[0]}");
                return -3;
            }

            return 0;
        }

        private static void InjectModLoader()
        {
            STModLoaderInjector.InjectModLoaderToSweetTransitDll(GameDirectoryManager.GetGameDirectoryPath());
        }

        private static void PatchDeps()
        {
            SweetTransitDepsJsonPatcher.PatchDepsJson(GameDirectoryManager.GetDepsJsonPath());
        }
    }
}