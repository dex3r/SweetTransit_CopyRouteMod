using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

            //var newInstruction = loadMethodProcessor.Create(OpCodes.Nop);

            //MethodDefinition exceptionConstructor =
            //    GetSystemRuntimeAssembly().GetType("System", "Exception").GetConstructors().Single(x => x.HasParameters == false);

            //var newObjInstr = loadMethodProcessor.Create(OpCodes.Newobj, exceptionConstructor);
            //var throwInstr = loadMethodProcessor.Create(OpCodes.Throw);

            //var reflectionAssembly = GetReflectionAssembly();
            //var assemblyType = reflectionAssembly.GetType("System.Reflection", "Assembly");
            //var assemblyMethods = assemblyType.Methods;
            //var assemblyLoadMethods = assemblyMethods.Where(x => x.Name == "Load");
            //var assemblyLoadMethod = assemblyLoadMethods.Single(x => x.Parameters.Count == 1 && x.Parameters[0].ParameterType.Name.ToLower() == "string");

            var assemblyLoadMethod = module.ImportReference(typeof(Assembly).GetMethod(nameof(Assembly.Load), new Type[] { typeof(string) }));
            
            var ldstr = loadMethodProcessor.Create(OpCodes.Ldstr, "nonexistingassembly");
            var call = loadMethodProcessor.Create(OpCodes.Call, assemblyLoadMethod);
            var pop = loadMethodProcessor.Create(OpCodes.Pop);
            
            var firstInstruction = loadMethod.Body.Instructions[0];

            loadMethodProcessor.InsertBefore(firstInstruction, ldstr);
            loadMethodProcessor.InsertBefore(firstInstruction, call);
            loadMethodProcessor.InsertBefore(firstInstruction, pop);
            
            //loadMethodProcessor.InsertBefore(firstInstruction, newObjInstr);
            //loadMethodProcessor.InsertBefore(firstInstruction, throwInstr);

            moddedAssemblyPath = Path.Combine(GameDirectoryManager.GetGameDirectoryPath(), "Sweet Transit.dll.modded");
            module.Write(moddedAssemblyPath);
            return stAssemblyPath;
        }

        private static ModuleDefinition GetSystemRuntimeAssembly()
        {
            string assemblyPath = Path.Combine(GameDirectoryManager.GetGameDirectoryPath(), "System.Runtime.dll");
            
            ModuleDefinition module = ModuleDefinition.ReadModule(assemblyPath);
            return module;
        }
        
        private static ModuleDefinition GetReflectionAssembly()
        {
            string assemblyPath = Path.Combine(GameDirectoryManager.GetGameDirectoryPath(), "System.Reflection.dll");
            
            ModuleDefinition module = ModuleDefinition.ReadModule(assemblyPath);
            return module;
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