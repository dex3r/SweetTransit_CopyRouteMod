using System;

namespace MatiModLoader
{
    public class ModLoaderException : Exception
    {
        public ModLoaderException(string message) : base(message)
        {
        }
        
        public ModLoaderException(string message, Exception baseException) : base(message, baseException)
        {
        }
    }
}