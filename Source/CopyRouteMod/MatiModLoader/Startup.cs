﻿using System;
using System.IO;

namespace MatiModLoader
{
    public static class Startup
    {
        public static void Initialize()
        {
            string logPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Sweet Transit",
                "MatiModLoader",
                "Log.txt");

            Directory.CreateDirectory(Path.GetDirectoryName(logPath));

            string initLine = $"MatiModLoader initialized at {DateTime.Now}";
            Console.WriteLine(initLine);
            File.AppendAllLines(logPath, new string[] { initLine });
            
            //throw new Exception("EXCEPTION!");
        }
    }
}