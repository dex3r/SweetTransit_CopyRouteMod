using System;
using System.IO;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace STModLoaderInjection
{
    public static class STModLoaderInjector
    {
        public static void InjectModLoaderToSweetTransitDll(string gameDirectoryPath)
        {
            string stAssemblyPath = Path.Combine(gameDirectoryPath, "Sweet Transit.dll");
            string moddedAssemblyPath = Path.Combine(gameDirectoryPath, "Sweet Transit.dll.modded");

            RevertOriginalAssembly(stAssemblyPath);
            
            CreateModdedAssembly(gameDirectoryPath, stAssemblyPath, moddedAssemblyPath);

            ReplaceOriginalAssembly(stAssemblyPath, moddedAssemblyPath);
            
            Console.WriteLine("Done. Sweet Transit dll has been modded with MatiModLoader.");
        }

        private static void CreateModdedAssembly(string gameDirectoryPath, string stAssemblyPath, string moddedAssemblyPath)
        {
            ReaderParameters readerParameters = new ReaderParameters(ReadingMode.Immediate);
            readerParameters.AssemblyResolver = new CustomResolver(gameDirectoryPath);

            using ModuleDefinition module = ModuleDefinition.ReadModule(stAssemblyPath, readerParameters);

            TypeDefinition loadingType = module.GetType("Sweet_Transit.GameData", "Loading");
            MethodDefinition loadMethod = loadingType.Methods.Single(x => x.Name == "Load");

            InjectLoadMethodPatch(loadMethod, module);

            module.Write(moddedAssemblyPath);
        }

        private static void InjectLoadMethodPatch(MethodDefinition loadMethod, ModuleDefinition module)
        {
            ILProcessor loadMethodProcessor = loadMethod.Body.GetILProcessor();
            
            MethodReference loadModMethod = module
                .ImportReference(typeof(MatiModLoader.MatiModLoader)
                    .GetMethod(nameof(MatiModLoader.MatiModLoader.LoadMod), new[] { typeof(string) }));

            // Load argument 0 (string path)
            Instruction loadArgument0 = loadMethodProcessor.Create(OpCodes.Ldarg_0);
            Instruction callLoadMod = loadMethodProcessor.Create(OpCodes.Call, loadModMethod);
            Instruction nop = loadMethodProcessor.Create(OpCodes.Nop);

            Instruction firstInstruction = loadMethod.Body.Instructions[0];
            loadMethodProcessor.InsertBefore(firstInstruction, loadArgument0);
            loadMethodProcessor.InsertBefore(firstInstruction, callLoadMod);
            loadMethodProcessor.InsertBefore(firstInstruction, nop);
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

            Console.WriteLine("Replacing assembly with modded one...");
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

            Console.WriteLine("Backing up original assembly...");
            File.Copy(originalAssemblyPath, backupAssemblyPath);
        }
        
        private static void RevertOriginalAssembly(string stAssemblyPath)
        {
            string backupAssemblyPath = $"{stAssemblyPath}.orig";

            if (File.Exists(backupAssemblyPath) == false)
            {
                Console.WriteLine("No assembly backup, assuming existing assembly is original");
                return;
            }

            Console.WriteLine("Reverting to original assembly...");
            File.Copy(backupAssemblyPath, stAssemblyPath, true);
        }
    }
}