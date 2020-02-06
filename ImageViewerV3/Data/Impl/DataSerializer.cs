using System.Collections.Generic;
using System.IO;
using System.Linq;
using DynamicData;
using ImageViewerV3.Ecs.Components;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ImageViewerV3.Data.Impl
{
    public class DataSerializer : IDataSerializer
    {
        private class GuardJObject
        {
            public JArray Array { get; } = new JArray();

            //private readonly HashSet<string> _guard = new HashSet<string>();

            public void AddSafe(string name, string value)
            {
                //if(!_guard.Add(name))
                //    throw new InvalidOperationException("Duplicate Key in JArray");

                Array.Add(
                    new JObject(
                        new JProperty("Category", name),
                        new JProperty("Value", value)));
            }
        }

        private const string TargetFileName = "_imageviewerdata.json";

        private readonly ISettingsDescriptor[] _blueprintDescriptors;

        private string _last = string.Empty;
        private ISourceList<DataComponent>? _lastCollection;

        public DataSerializer(ISettingsDescriptor[] blueprintDescriptors) 
            => _blueprintDescriptors = blueprintDescriptors;

        public void LoadFrom(string path, ISourceList<DataComponent> to)
        {
            _lastCollection = to;
            _last = path;

            var targetPath = GetFullPath(path);

            if (!File.Exists(targetPath))
                return;

            try
            {
                JObject obj;

                try
                {
                    obj = JObject.Parse(File.ReadAllText(targetPath));
                }
                catch
                {
                    obj = JObject.Parse(File.ReadAllText(targetPath + 1));
                }

                to.AddRange(from property in obj.Properties()
                    let blue = _blueprintDescriptors.FirstOrDefault(bd => bd.Category == property.Name)
                               ?? _blueprintDescriptors.First(bd => bd.Category == "general")
                    let jValue = (JArray) property.Value
                    from entry in jValue
                    select blue.Create(entry.Value<string>("Category"), entry.Value<string>("Value")));
            }
            catch 
            {
                try
                {
                    File.Delete(targetPath);
                }
                catch(IOException)
                {
                    
                }
            }
        }

        public void Save()
        {
            if(_lastCollection == null || string.IsNullOrWhiteSpace(_last)) return;
            var path = GetFullPath(_last);

            var types = new Dictionary<string, GuardJObject>();
            
            foreach (var ent in _lastCollection.Items)
            {
                var type = ent.Category ?? "general";

                if (!types.TryGetValue(type, out var array))
                {
                    array = new GuardJObject();
                    types[type] = array;
                }

                array.AddSafe(ent.Name, ent.ReactiveValue.Value);
            }

            var obj = new JObject(types.Select(kp => new JProperty(kp.Key, kp.Value.Array)));

            if(File.Exists(path))
                File.Copy(path, path + 1, true);

            File.WriteAllText(path, obj.ToString(Formatting.Indented));
        }

        private static string GetFullPath(string location) 
            => location.EndsWith(TargetFileName) ? location : Path.Combine(location, TargetFileName);
    }
}