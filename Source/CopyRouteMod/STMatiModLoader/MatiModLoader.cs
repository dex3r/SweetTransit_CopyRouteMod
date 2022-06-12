using System;
using System.IO;
using System.Reflection;

namespace STMatiModLoader
{
    public static class MatiModLoader
    {
        private const string MatiModInterfaceTypeName = "IMatiMod";
        private const string MatiModInterfaceLoadMethodName = "Load";
        
        public static void LoadMod(string modRootDirectory)
        {
            foreach (string dllPath in Directory.GetFiles(modRootDirectory, "*.dll", SearchOption.AllDirectories))
            {
                TryToLoadDll(dllPath);
            }
        }

        private static void TryToLoadDll(string dllPath)
        {
            Assembly loadedAssembly;
            
            try
            {
                loadedAssembly = AppDomain.CurrentDomain.Load(dllPath);
            }
            catch (Exception e)
            {
                throw new ModLoaderException($"Failed to load dotnet assembly under path '{dllPath}'", e);
            }

            InitializeMod(loadedAssembly);
        }

        private static void InitializeMod(Assembly modAssembly)
        {
            bool anyFound = false;
            
            foreach (TypeInfo typeInfo in modAssembly.DefinedTypes)
            {
                if (TryInitializeModType(typeInfo))
                {
                    anyFound = true;
                }
            }

            if (!anyFound)
            {
                throw new ModLoaderException(
                    $"Loaded mod could not be initialized: no public classes implementing '{MatiModInterfaceTypeName}' found. At least one is required");
            }
        }

        private static bool TryInitializeModType(TypeInfo typeInfo)
        {
            Type modType = typeInfo.GetInterface(MatiModInterfaceTypeName);

            if (modType == null)
            {
                return false;
            }

            object modObject;
            
            try
            {
                modObject = Activator.CreateInstance(modType);
            }
            catch (Exception e)
            {
                throw new ModLoaderException($"Failed to create instance of mod type '{typeInfo.FullName}'", e);
            }

            MethodInfo loadMethod = modType.GetMethod(MatiModInterfaceLoadMethodName, BindingFlags.Instance | BindingFlags.Public);

            if (loadMethod == null)
            {
                throw new ModLoaderException($"Could not find method '{MatiModInterfaceLoadMethodName}' in mod type '{typeInfo.FullName}'");
            }

            ParameterInfo[] loadMethodDeclarationParams = loadMethod.GetParameters();

            if (loadMethodDeclarationParams.Length != 0)
            {
                throw new ModLoaderException($"Method '{MatiModInterfaceLoadMethodName}' in mod type '{typeInfo.FullName}' does not have 0 arguments.");
            }

            try
            {
                loadMethod.Invoke(modObject, new object?[0]);
            }
            catch (Exception e)
            {
                throw new ModLoaderException($"Mod '{typeInfo.Assembly.FullName}' loading method threw an exception. " +
                                             $"Method: {modType.FullName}.{loadMethod.Name}", e);
            }

            return true;
        }
    }
}