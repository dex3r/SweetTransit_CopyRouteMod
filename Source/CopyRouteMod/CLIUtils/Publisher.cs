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

        private void Publish_Internal()
        {
            string baseDirectory = Path.Combine(Environment.CurrentDirectory, "../../../../");
            CheckBaseDirectory(baseDirectory);
            
            string outputDirectory = Path.Combine(baseDirectory, "Output");
            string outputTemplateDirectory = Path.Combine(baseDirectory, "Output_template");

            Console.WriteLine("Cleaning output directory...");
            Directory.Delete(outputDirectory, true);
            Directory.CreateDirectory(outputDirectory);

            Console.WriteLine("Copying Output Template to Output...");
            CopyAllFiles(outputTemplateDirectory, outputDirectory);
        }

        private static void CopyAllFiles(string sourceDirPath, string targetDirPath)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(sourceDirPath);

            if (sourceDir.Exists == false)
            {
                throw new Exception("Failed to copy files: source directory does not exist");
            }
            
            CopyAllFiles(sourceDir, new DirectoryInfo(targetDirPath));
        }
        
        private static void CopyAllFiles(DirectoryInfo source, DirectoryInfo target) 
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyAllFiles(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name));
            }
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