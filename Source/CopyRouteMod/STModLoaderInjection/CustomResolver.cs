using System;
using System.IO;
using Mono.Cecil;

namespace STModLoaderInjection
{
    public class CustomResolver : BaseAssemblyResolver
    {
        private readonly string _searchDirectoryPath;
        private readonly DefaultAssemblyResolver _defaultResolver;

        public CustomResolver(string searchDirectoryPath)
        {
            _searchDirectoryPath = searchDirectoryPath;
            _defaultResolver = new DefaultAssemblyResolver();
        }

        public override AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            AssemblyDefinition assembly;
            try
            {
                assembly = _defaultResolver.Resolve(name);
            }
            catch (AssemblyResolutionException ex)
            {
                Console.WriteLine("Trying to resolve " + name.FullName);
                assembly = TryToResolveAssembly(name);
            }
            
            return assembly;
        }

        private AssemblyDefinition TryToResolveAssembly(AssemblyNameReference name)
        {
            string path = Path.Combine(_searchDirectoryPath, $"{name.Name}.dll");

            if (File.Exists(path) == false)
            {
                throw new Exception($"Could not find assembly at path '{path}'");
            }
            
            AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(path);

            return assemblyDefinition;
        }
    }
}