using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Cecilifier.Runtime;
using CLIUtils;
using Mono.Cecil;
using Mono.Cecil.Cil;
using BindingFlags = System.Reflection.BindingFlags;
using MethodAttributes = Mono.Cecil.MethodAttributes;
using ParameterAttributes = Mono.Cecil.ParameterAttributes;

namespace STModLoaderInjection
{
    class STModLoaderInjectionProgram
    {
        static void Main(string[] args)
        {
            string stAssemblyPath = Path.Combine(GameDirectoryManager.GetGameDirectoryPath(), "Sweet Transit.dll");
            string moddedAssemblyPath = Path.Combine(GameDirectoryManager.GetGameDirectoryPath(), "Sweet Transit.dll.modded");

            RevertOriginalAssembly(stAssemblyPath);
            
            CreateModdedAssembly(stAssemblyPath, moddedAssemblyPath);

            ReplaceOriginalAssembly(stAssemblyPath, moddedAssemblyPath);
            
            Console.WriteLine("Done. Sweet Transit dll has been modded with MatiModLoader.");
        }

        private static void CreateModdedAssembly(string stAssemblyPath, string moddedAssemblyPath)
        {
            ReaderParameters readerParameters = new ReaderParameters(ReadingMode.Immediate);
            readerParameters.AssemblyResolver = new CustomResolver(GameDirectoryManager.GetGameDirectoryPath());

            using ModuleDefinition module = ModuleDefinition.ReadModule(stAssemblyPath, readerParameters);

            TypeDefinition loadingType = module.GetType("Sweet_Transit.GameData", "Loading");
            MethodDefinition loadMethod = loadingType.Methods.Single(x => x.Name == "Load");

            ILProcessor loadMethodProcessor = loadMethod.Body.GetILProcessor();
            var firstInstruction = loadMethod.Body.Instructions[0];

            //var assemblyLoadMethod = module.ImportReference(typeof(Assembly).GetMethod(nameof(Assembly.LoadFrom), new Type[] { typeof(string) }));
            
            //var ldstr = loadMethodProcessor.Create(OpCodes.Ldstr, "Data/MatiModLoader/netcoreapp3.1/MatiModLoader.dll");
            //var call = loadMethodProcessor.Create(OpCodes.Call, assemblyLoadMethod);
            //var pop = loadMethodProcessor.Create(OpCodes.Pop);
            //
//
            //loadMethodProcessor.InsertBefore(firstInstruction, ldstr);
            //loadMethodProcessor.InsertBefore(firstInstruction, call);
            //loadMethodProcessor.InsertBefore(firstInstruction, pop);
//
            //var loadModMethod = module
            //    .ImportReference(typeof(MatiModLoader.MatiModLoader)
            //        .GetMethod(nameof(MatiModLoader.MatiModLoader.LoadMod), new []{typeof(string)}));
//
            //var ldarg0 = loadMethodProcessor.Create(OpCodes.Ldarg_0);
            //var callLoadMod = loadMethodProcessor.Create(OpCodes.Call, loadModMethod);
            //var nop = loadMethodProcessor.Create(OpCodes.Nop);
//
            //loadMethodProcessor.InsertBefore(firstInstruction, ldarg0);
            //loadMethodProcessor.InsertBefore(firstInstruction, callLoadMod);
            //loadMethodProcessor.InsertBefore(firstInstruction, nop);

            MethodDefinition methodToCall = AddMatiLoadModMethod(module.Assembly, loadingType);

            var ldarg0 = loadMethodProcessor.Create(OpCodes.Ldarg_0);
            var callLoadMod = loadMethodProcessor.Create(OpCodes.Call, methodToCall);
            var nop = loadMethodProcessor.Create(OpCodes.Nop);
            
            loadMethodProcessor.InsertBefore(firstInstruction, ldarg0);
            loadMethodProcessor.InsertBefore(firstInstruction, callLoadMod);
            loadMethodProcessor.InsertBefore(firstInstruction, nop);

            module.Write(moddedAssemblyPath);
        }

        private static MethodDefinition AddMatiLoadModMethod(AssemblyDefinition assembly, TypeDefinition loadingType)
        {
	        //Method : MatiLoadMod
			var m_MatiLoadMod_1 = new MethodDefinition("MatiLoadMod", MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.HideBySig, assembly.MainModule.TypeSystem.Void);
			loadingType.Methods.Add(m_MatiLoadMod_1);
			m_MatiLoadMod_1.Body.InitLocals = true;
			var il_MatiLoadMod_2 = m_MatiLoadMod_1.Body.GetILProcessor();

			//Parameters of 'public static void MatiLoadMod(string path)'
			var p_path_3 = new ParameterDefinition("path", ParameterAttributes.None, assembly.MainModule.TypeSystem.String);
			m_MatiLoadMod_1.Parameters.Add(p_path_3);

			//Assembly assembly = Assembly.LoadFrom("Data/MatiModLoader/netcoreapp3.1/MatiModLoader.dll");
			var lv_assembly_4 = new VariableDefinition(assembly.MainModule.ImportReference(typeof(System.Reflection.Assembly)));
			m_MatiLoadMod_1.Body.Variables.Add(lv_assembly_4);
			il_MatiLoadMod_2.Emit(OpCodes.Ldstr, "Data/MatiModLoader/netcoreapp3.1/MatiModLoader.dll");
			il_MatiLoadMod_2.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod("System.Reflection.Assembly", "LoadFrom",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public,"", "System.String")));
			il_MatiLoadMod_2.Emit(OpCodes.Stloc, lv_assembly_4);

			//Type type = assembly.GetType("MatiModLoader.MatiModLoader");
			var lv_type_6 = new VariableDefinition(assembly.MainModule.ImportReference(typeof(System.Type)));
			m_MatiLoadMod_1.Body.Variables.Add(lv_type_6);
			il_MatiLoadMod_2.Emit(OpCodes.Ldloc, lv_assembly_4);
			il_MatiLoadMod_2.Emit(OpCodes.Ldstr, "MatiModLoader.MatiModLoader");
			il_MatiLoadMod_2.Emit(OpCodes.Callvirt, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod("System.Reflection.Assembly", "GetType",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public,"", "System.String")));
			il_MatiLoadMod_2.Emit(OpCodes.Stloc, lv_type_6);

			//MethodInfo method = type.GetMethod("LoadMod", BindingFlags.Static | BindingFlags.Public);
			var lv_method_8 = new VariableDefinition(assembly.MainModule.ImportReference(typeof(System.Reflection.MethodInfo)));
			m_MatiLoadMod_1.Body.Variables.Add(lv_method_8);
			il_MatiLoadMod_2.Emit(OpCodes.Ldloc, lv_type_6);
			il_MatiLoadMod_2.Emit(OpCodes.Ldstr, "LoadMod");
			//il_MatiLoadMod_2.Emit(OpCodes.Ldnull);
			//il_MatiLoadMod_2.Emit(OpCodes.Ldnull);
			il_MatiLoadMod_2.Emit(OpCodes.Ldc_I4_S, (sbyte)24);
			//il_MatiLoadMod_2.Emit(OpCodes.Call, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod("System.Reflection.BindingFlags", "op_BitwiseOr",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.Public,"", "System.Reflection.BindingFlags", "System.Reflection.BindingFlags")));
			il_MatiLoadMod_2.Emit(OpCodes.Callvirt, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod("System.Type", "GetMethod",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public,"", "System.String", "System.Reflection.BindingFlags")));
			il_MatiLoadMod_2.Emit(OpCodes.Stloc, lv_method_8);

			//method.Invoke(null, new[] { path });
			il_MatiLoadMod_2.Emit(OpCodes.Ldloc, lv_method_8);
			il_MatiLoadMod_2.Emit(OpCodes.Ldnull);
			il_MatiLoadMod_2.Emit(OpCodes.Ldc_I4, 1);
			il_MatiLoadMod_2.Emit(OpCodes.Newarr, assembly.MainModule.TypeSystem.String);
			il_MatiLoadMod_2.Emit(OpCodes.Dup);
			il_MatiLoadMod_2.Emit(OpCodes.Ldc_I4, 0);
			il_MatiLoadMod_2.Emit(OpCodes.Ldarg_0);
			il_MatiLoadMod_2.Emit(OpCodes.Stelem_Ref);
			il_MatiLoadMod_2.Emit(OpCodes.Callvirt, assembly.MainModule.ImportReference(TypeHelpers.ResolveMethod("System.Reflection.MethodBase", "Invoke",System.Reflection.BindingFlags.Default|System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public,"", "System.Object", "System.Object[]")));
			il_MatiLoadMod_2.Emit(OpCodes.Pop);
			il_MatiLoadMod_2.Emit(OpCodes.Ret);

			return m_MatiLoadMod_1;
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