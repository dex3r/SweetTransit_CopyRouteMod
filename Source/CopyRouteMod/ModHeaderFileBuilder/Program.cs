using System;
using System.IO;
using System.Reflection;
using STVisual.DefReader;
using STVisual.IO;
using Sweet_Transit.GameData;

namespace ModHeaderFileBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            string fileName = "Header.txt";
            string directoryPath = Path.Combine(Environment.CurrentDirectory, "../../../../Output/CopyRouteMod");
            string outputPath = Path.Combine(directoryPath, fileName);
            outputPath = Path.GetFullPath(outputPath);

            Directory.CreateDirectory(directoryPath);
            
            //SaveModHeader(GetCopyRouteModHeader(), outputPath);

            LoadModHeader(outputPath);
        }

        private static void LoadModHeader(string outputPath)
        {
            string directoryPath = Path.GetDirectoryName(outputPath);
            DefValueArray defValueArray = DefValueParser.Get(outputPath);

            ModItem modItem = new ModItem(directoryPath, defValueArray, true);
            
            Console.WriteLine("Read Header.txt:");
            PrintModItem(modItem);
        }

        private static DefValueArray GetCopyRouteModHeader()
        {
            string modItemPath = "";
            
            DefValueArray modHeaderDefValueArray = new DefValueArray();
            
            modHeaderDefValueArray
                .GetType()
                .GetField("name", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .SetValue(modHeaderDefValueArray, "File Parent");

            DefValue modNameValue = new DefValue
            {
                name = "name",
                value = "CopyRouteMod"
            };

            modHeaderDefValueArray.AddOrReplaceValue(modNameValue);
            AddValue(modHeaderDefValueArray, "name", "ModHeaderBuilder");
            AddValue(modHeaderDefValueArray, "mod_version", "0,0,2");

            ModItem modItem = new ModItem(modItemPath, modHeaderDefValueArray, true);
            
            Console.WriteLine("Output mod item:");
            PrintModItem(modItem);

            return modHeaderDefValueArray;
        }

        private static void PrintModItem(ModItem modItem)
        {
            Console.WriteLine($"Mod name: {modItem.Name}");
            Console.WriteLine($"Mod version: {modItem.Mod_version}");
        }

        private static void SaveModHeader(DefValueArray modHeaderDefValueArray, string path)
        {
            SavingStream savingStream = new SavingStream();
            modHeaderDefValueArray.GetData(savingStream);

            byte[] bytes = savingStream.GetBytes();

            File.WriteAllBytes(path, bytes);
           //File.WriteAllText(path,headerText);
        }

        private static void AddValue(DefValueArray defValueArray, string key, string value)
        {
            defValueArray.AddOrReplaceValue(new DefValue
            {
                name = key,
                value = value
            });
        }
    }
}