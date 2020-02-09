using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using Newtonsoft.Json;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var setting = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented};

            var test = new List<Type>
                       {
                           typeof(string),
                           typeof(int),
                           typeof(bool)
                       };

            var testJson = JsonConvert.SerializeObject(test, setting);

            test = JsonConvert.DeserializeObject<List<Type>>(testJson);

            Console.WriteLine("Fertig");
            Console.ReadKey();
        }
    }
}
