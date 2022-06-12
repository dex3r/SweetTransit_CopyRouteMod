using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using CLIUtils;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace STModLoaderInjection
{
    class STModLoaderInjectionProgram
    {
        static void Main(string[] args)
        {
            string stAssemblyPath = Path.Combine(GameDirectoryManager.GetGameDirectoryPath(), "Sweet Transit.dll");

            ReaderParameters readerParameters = new ReaderParameters(ReadingMode.Immediate);
            readerParameters.AssemblyResolver = new CustomResolver(GameDirectoryManager.GetGameDirectoryPath());
            
            ModuleDefinition module = ModuleDefinition.ReadModule(stAssemblyPath, readerParameters);

            TypeDefinition loadingType = module.GetType("Sweet_Transit.GameData", "Loading");
            MethodDefinition loadMethod = loadingType.Methods.Single(x => x.Name == "Load");

            ILProcessor loadMethodProcessor = loadMethod.Body.GetILProcessor();

            //var loadModMethod = module.Import(typeof(MatiModLoader).GetMethod(nameof(MatiModLoader.LoadMod)));
            //var callModLoaderInstruction = loadMethodProcessor.Create(OpCodes.Call, loadModMethod);

            var newInstruction = loadMethodProcessor.Create(OpCodes.Nop);
                
            var firstInstruction = loadMethod.Body.Instructions[0];
            loadMethodProcessor.InsertBefore(firstInstruction, newInstruction);
            
            module.Write("Sweet Transit.dll Modded");
            
            Console.WriteLine("Done. Sweet Transit dll has been modded with MatiModLoader.");
        }
    }
}