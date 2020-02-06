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

namespace TestApp
{
    class Program
    {
        private class TestImage
        {
            public string FilePath { get; }

            public string FileName => Path.GetFileName(FilePath);

            public TestImage(string filePath) => FilePath = filePath;
        }

        private class TestImage2
        {
            public string FilePath { get; }

            public string FileName => Path.GetFileName(FilePath);

            public TestImage2(string filePath) => FilePath = filePath;
        }

        static void Main(string[] args)
        {
            const string testPath = @"G:\Shankaku";

            var cache = new SourceList<TestImage>();
            IObservableCollection<TestImage2> img = new ObservableCollectionExtended<TestImage2>();
            
            cache.Connect().Filter(ti => ti.FileName.Contains('9')).Select(ti => new TestImage2(ti.FilePath)).Bind(img).Subscribe();
            
            cache.AddRange(Directory.EnumerateFiles(testPath).OrderBy(s => s, StringComparer.Ordinal).Select(s => new TestImage(s)));

            Console.Write(img.Count);

            Console.WriteLine("Fertig");
            Console.ReadKey();
        }
    }
}
