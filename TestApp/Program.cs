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
using ImageViewerV3.Ui.Services;
using Newtonsoft.Json;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var testQuery = SearchQuery.ParseTerm("favorite:true date:b30.01.2017;20.01.2020; rating:>=3 -test test2 test_3");

            Console.WriteLine("Fertig");
            Console.ReadKey();
        }
    }
}
