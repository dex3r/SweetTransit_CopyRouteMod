using System;
using System.Diagnostics;
using System.IO;

namespace CLIUtils
{
    public static class GameDirectoryManager
    {
        public static string GetGameDirectoryPath() => Path.GetFullPath(Path.Combine(Publisher.GetProjectBaseDirectory(), "../SweetTransit"));

        public static void CopyModToGame()
        {
            string modPublishOutputDir = Publisher.GetOutputDirectory();

            if (Directory.Exists(modPublishOutputDir) == false)
            {
                throw new Exception($"Failed to copy mod to game directory. Mod output does not exist under {modPublishOutputDir}");
            }

            string gameDirectory = GetGameDirectoryPath();

            if (Directory.Exists(gameDirectory) == false)
            {
                throw new Exception($"Failed to copy mod to game directory. Game directory does not exist under {gameDirectory}");
            }

            string dataDirectory = Path.Combine(gameDirectory, "Data");

            Console.WriteLine("Copying mod output to game directory...");
            IoUtils.CopyAllFiles(modPublishOutputDir, dataDirectory, true);
        }

        public static void LaunchGame()
        {
            string gameDirectory = GetGameDirectoryPath();
            string gameExePath = Path.Combine(gameDirectory, "Sweet Transit.exe");

            Console.WriteLine("Launching game...");
            Process gameProcess = new Process();
            gameProcess.StartInfo = new ProcessStartInfo
            {
                WorkingDirectory = gameDirectory,
                FileName = gameExePath
            };

            gameProcess.Start();

            if(gameProcess == null)
            {
                throw new Exception("Failed to launch the game");
            }
            
            Console.WriteLine("Game launched. Waiting for exit...");
            gameProcess.WaitForExit();
            
            Console.WriteLine("Game exited with code: " + gameProcess.ExitCode);
        }
    }
}