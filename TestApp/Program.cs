using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TestApp
{
    class Program
    {
        private class Test
        {
            public string Path { get; }

            public Test(string path) => Path = path;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Fertig");
            Console.ReadKey();
        }
    }
}
