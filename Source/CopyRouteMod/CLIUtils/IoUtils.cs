using System;
using System.IO;

namespace CLIUtils
{
    public static class IoUtils
    {
        public static void CopyAllFiles(string sourceDirPath, string targetDirPath, bool overrideFiles = false)
        {
            DirectoryInfo sourceDir = new DirectoryInfo(sourceDirPath);

            if (sourceDir.Exists == false)
            {
                throw new Exception("Failed to copy files: source directory does not exist");
            }
            
            CopyAllFiles(sourceDir, new DirectoryInfo(targetDirPath), overrideFiles);
        }
        
        public static void CopyAllFiles(DirectoryInfo source, DirectoryInfo target, bool overrideFiles = false) 
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyAllFiles(dir, target.CreateSubdirectory(dir.Name), overrideFiles);
            }

            foreach (FileInfo file in source.GetFiles())
            {
                file.CopyTo(Path.Combine(target.FullName, file.Name), overrideFiles);
            }
        }
    }
}