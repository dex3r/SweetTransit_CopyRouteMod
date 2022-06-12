using System;
using System.Reflection;

namespace STModLoaderInjection
{
    public static class CallTemplate
    {
        public static void Throw()
        {
            Assembly.Load("dasdasd");
        }
    }
}