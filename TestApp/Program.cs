using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            const string testPath = @"C:\Program Files\Microsoft Office 15\root\office15\outlook.exe";

            using var process = Process.Start(testPath);

            Console.WriteLine("Ready to test");
            Console.ReadKey();

            Console.WriteLine(process?.CloseMainWindow());

            Console.WriteLine("Fertig");
            Console.ReadKey();
        }
    }
}
