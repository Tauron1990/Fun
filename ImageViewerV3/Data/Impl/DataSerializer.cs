using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EcsRx.Collections;
using EcsRx.Extensions;
using ImageViewerV3.Ecs.Components.Data;
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
                        new JProperty("Name", name),
                        new JProperty("Value", value)));
            }
        }

        private const string TargetFileName = "_imageviewerdata.json";

        private readonly IBlueprintDescriptor[] _blueprintDescriptors;

        private string _last = string.Empty;
        private IEntityCollection? _lastCollection;

        public DataSerializer(IBlueprintDescriptor[] blueprintDescriptors) 
            => _blueprintDescriptors = blueprintDescriptors;

        public void LoadFrom(string path, IEntityCollection to)
        {
            _lastCollection = to;
            _last = path;

            var targetPath = GetFullPath(path);

            if (!File.Exists(targetPath))
                return;


            try
            {
                var obj = JObject.Parse(File.ReadAllText(targetPath));
                foreach (var property in obj.Properties())
                {
                    var blue = _blueprintDescriptors.FirstOrDefault(bd => bd.Name == property.Name) 
                               ?? _blueprintDescriptors.First(bd => bd.Name == "general");

                    var jValue = (JArray) property.Value;
                    var blueprint = blue.Create(jValue.First.Value<string>("Name"), jValue.First.Value<string>("Value"));

                    to.CreateEntity(blueprint);
                }
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
            
            foreach (var ent in _lastCollection)
            {
                var type = "general";
                if (ent.HasComponent<TypeComponent>())
                    type = ent.GetComponent<TypeComponent>().Name ?? "general";

                if (!types.TryGetValue(type, out var array))
                {
                    array = new GuardJObject();
                    types[type] = array;
                }

                var data = ent.GetComponent<DataComponent>();
                array.AddSafe(data.Name, data.ReactiveValue.Value);
            }

            var obj = new JObject(types.Select(kp => new JProperty(kp.Key, kp.Value.Array)));

            File.WriteAllText(path, obj.ToString(Formatting.Indented));
        }

        private static string GetFullPath(string location) 
            => location.EndsWith(TargetFileName) ? location : Path.Combine(location, TargetFileName);
    }
}