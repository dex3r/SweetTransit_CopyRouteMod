using System;

namespace CLIUtils
{
    class CLIUtilsProgram
    {
        static int Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Expected more than 0 arguments");
                return -2;
            }

            if (args[0].ToLower() == "publish")
            {
                Publisher.Publish();
                return 0;
            }
            
            Console.WriteLine($"Unknown argument {args[0]}");
            return -3;
        }
    }
}