using System;
using System.Reflection;

namespace STModLoaderInjection
{
    public static class CallTemplate
    {
        public static void Throw(string path)
        {
            Assembly assembly = Assembly.Load("dasdasd");
            Type type = assembly.GetType("MatiModLoader.MatiModLoader");
            MethodInfo method = type.GetMethod("LoadMod", BindingFlags.Static | BindingFlags.Public);
            method.Invoke(null, new[] { path });
        }

        public static void Main(string path)
        {
            Throw(path);
        }
    }
}