using System;
using System.IO;
using System.Linq;
using CLIUtils;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace STModLoaderInjection
{
    class STModLoaderInjectionProgram
    {
        static void Main(string[] args)
        {
            string stAssemblyPath = CreateModdedAssembly(out string moddedAssemblyPath);

            ReplaceOriginalAssembly(stAssemblyPath, moddedAssemblyPath);
            
            Console.WriteLine("Done. Sweet Transit dll has been modded with MatiModLoader.");
        }

        private static string CreateModdedAssembly(out string moddedAssemblyPath)
        {
            string stAssemblyPath = Path.Combine(GameDirectoryManager.GetGameDirectoryPath(), "Sweet Transit.dll");

            ReaderParameters readerParameters = new ReaderParameters(ReadingMode.Immediate);
            readerParameters.AssemblyResolver = new CustomResolver(GameDirectoryManager.GetGameDirectoryPath());

            using ModuleDefinition module = ModuleDefinition.ReadModule(stAssemblyPath, readerParameters);

            TypeDefinition loadingType = module.GetType("Sweet_Transit.GameData", "Loading");
            MethodDefinition loadMethod = loadingType.Methods.Single(x => x.Name == "Load");

            ILProcessor loadMethodProcessor = loadMethod.Body.GetILProcessor();

            //var loadModMethod = module.Import(typeof(MatiModLoader).GetMethod(nameof(MatiModLoader.LoadMod)));
            //var callModLoaderInstruction = loadMethodProcessor.Create(OpCodes.Call, loadModMethod);

            var newInstruction = loadMethodProcessor.Create(OpCodes.Nop);

            var firstInstruction = loadMethod.Body.Instructions[0];
            loadMethodProcessor.InsertBefore(firstInstruction, newInstruction);

            moddedAssemblyPath = Path.Combine(GameDirectoryManager.GetGameDirectoryPath(), "Sweet Transit.dll.modded");
            module.Write(moddedAssemblyPath);
            return stAssemblyPath;
        }

        private static void ReplaceOriginalAssembly(string originalAssemblyPath, string moddedAssemblyPath)
        {
            if (File.Exists(originalAssemblyPath) == false)
            {
                throw new Exception("Original assembly not found.");
            }
            
            if(File.Exists(moddedAssemblyPath) == false)
            {
                throw new Exception("Modded assembly not found.");
            }

            TryToCreateAssemblyBackup(originalAssemblyPath);

            File.Copy(moddedAssemblyPath, originalAssemblyPath, true);
            File.Delete(moddedAssemblyPath);
        }

        private static void TryToCreateAssemblyBackup(string originalAssemblyPath)
        {
            string backupAssemblyPath = $"{originalAssemblyPath}.orig";

            if (File.Exists(backupAssemblyPath))
            {
                return;
            }

            File.Copy(originalAssemblyPath, backupAssemblyPath);
        }
    }
}