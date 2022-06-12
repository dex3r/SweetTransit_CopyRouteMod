using System;
using System.IO;

namespace CLIUtils
{
    public class Publisher
    {
        public static void Publish()
        {
            new Publisher().Publish_Internal();
        }

        public static string GetProjectBaseDirectory()
        {
            string baseDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../");
            CheckBaseDirectory(baseDirectory);

            return baseDirectory;
        }
        
        public static string GetOutputDirectory()
        {
            string baseDirectory = GetProjectBaseDirectory();
            string outputDirectory = Path.Combine(baseDirectory, "Output");
            return outputDirectory;
        }
        
        private void Publish_Internal()
        {
            string baseDirectory = GetProjectBaseDirectory();
            string outputDirectory = GetOutputDirectory();
            string outputTemplateDirectory = Path.Combine(baseDirectory, "Output_template");

            Console.WriteLine("Cleaning output directory...");
            Directory.Delete(outputDirectory, true);
            Directory.CreateDirectory(outputDirectory);

            Console.WriteLine("Copying Output Template to Output...");
            IoUtils.CopyAllFiles(outputTemplateDirectory, outputDirectory);
        }

        private static void CheckBaseDirectory(string baseDirectoryPath)
        {
            string baseDirectory = Path.Combine(baseDirectoryPath, "Source");
            if (Directory.Exists(baseDirectory) == false)
            {
                // Sanity check - break if program ended up in wrong directory
                throw new Exception($"Unexpected output directory path: '{baseDirectoryPath}' Full: {Path.GetFullPath(baseDirectoryPath)}");
            }
        }
    }
}