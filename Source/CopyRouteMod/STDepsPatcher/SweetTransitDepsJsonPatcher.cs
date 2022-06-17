using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace STDepsPatcher
{
    public static class SweetTransitDepsJsonPatcher
    {
        public static void PatchDepsJson(string depsFilePath)
        {
            Console.WriteLine($"Patching DepsJson under '{depsFilePath}'...");
            
            RestoreOrCreateDepsJsonBackupFile(depsFilePath);

            byte[] originalFileContent = File.ReadAllBytes(depsFilePath);
            using MemoryStream originalFileStream = new MemoryStream(originalFileContent);
            using StreamReader originalFileReader = new StreamReader(originalFileStream);

            using (StreamWriter targetFileStreamWriter = new StreamWriter(depsFilePath, false))
            {
                PatchDepsJsonFile(originalFileReader, targetFileStreamWriter);
            }

            Console.WriteLine("DepsJson patched.");
        }
        
        public static void RestoreOrCreateDepsJsonBackupFile(string depsFilePath)
        {
            if (File.Exists(depsFilePath) == false)
            {
                throw new Exception($"Could not find deps file under path: {depsFilePath}");
            }
            
            string backupFilePath = $"{depsFilePath}.orig";

            if (File.Exists(backupFilePath))
            {
                File.Copy(backupFilePath, depsFilePath, true);
                Console.WriteLine("DepsJson backup restored.");
                return;
            }

            File.Copy(depsFilePath, backupFilePath, false);
        }
        
        public static void PatchDepsJsonFile(TextReader readStream, StreamWriter outputStream)
        {
            JsonTextReader reader = new JsonTextReader(readStream);
            JObject jObject = JObject.Load(reader);

            PatchTargets(jObject);
            PatchLibraries(jObject);

            JsonWriter jsonWriter = new JsonTextWriter(outputStream);
            jsonWriter.Formatting = Formatting.Indented;
            jObject.WriteTo(jsonWriter);
        }

        private static void PatchLibraries(JObject jObject)
        {
            JObject librariesPath = (JObject)jObject["libraries"];
            librariesPath["MatiModLoader/1.0.0"] = CreateMatiModLoaderLibrariesObject();
        }

        private static JToken CreateMatiModLoaderLibrariesObject()
        {
            JObject jObject = new JObject();
            
            jObject["type"] = new JValue("package");
            jObject["serviceable"] = new JValue(true);
            jObject["sha512"] = new JValue("");
            
            return jObject;
        }

        private static void PatchTargets(JObject jObject)
        {
            JObject targetsPath = (JObject)jObject["targets"][".NETCoreApp,Version=v3.1/win-x64"];
            targetsPath["MatiModLoader/1.0.0"] = CreateMatiModLoaderTargetsObject();
        }

        private static JObject CreateMatiModLoaderTargetsObject()
        {
            JObject jObject = new JObject();
            JObject runtimeToken = new JObject();
            JObject dllToken = new JObject();

            runtimeToken["MatiModLoader.dll"] = dllToken;
            jObject["runtime"] = runtimeToken;

            return jObject;
        }
    }
}