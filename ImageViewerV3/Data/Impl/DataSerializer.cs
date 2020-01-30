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

            var obj = JObject.Parse(File.ReadAllText(targetPath));
            foreach (var property in obj.Properties())
            {
                var blue = _blueprintDescriptors.FirstOrDefault(bd => bd.Name == property.Name) 
                           ?? _blueprintDescriptors.First(bd => bd.Name == "general");

                var jValue = (JObject) property.Value;
                var blueprint = blue.Create(jValue.Value<string>("Name"), jValue.Value<string>("Value"));

                to.CreateEntity(blueprint);
            }
        }

        public void Save()
        {
            if(_lastCollection == null) return;
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
            using var stream = new FileStream(path, FileMode.Create);
            obj.WriteTo(new JsonTextWriter(new StreamWriter(stream)));
        }

        private static string GetFullPath(string location) 
            => location.EndsWith(TargetFileName) ? location : Path.Combine(location, TargetFileName);
    }
}