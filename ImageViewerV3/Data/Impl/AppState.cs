using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;

namespace ImageViewerV3.Data.Impl
{
    public sealed class AppState : IAppState
    {
        private const string Applocation = "Tauron\\ImnageViewerV3\\appstate.json";
        
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings{ TypeNameHandling = TypeNameHandling.Auto, Formatting = Formatting.Indented };
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Applocation);

        private Dictionary<Type, object> _states = new Dictionary<Type, object>();
        
        public AppState() 
            => LoadObject();

        public TType Get<TType>()
            where TType : new()
        {
            var type = typeof(TType);
            if (_states.TryGetValue(type, out var state))
                return (TType) state;

            return new TType();
        }

        public void Set<TType>(Action<TType> updater)
            where TType : new()
        {
            if (updater == null)
                return;

            var element = Get<TType>();
            updater(element);

            if (element == null)
                return;
            
            _states[typeof(TType)] = element;
            SaveObject();
        }

        private void SaveObject()
        {
            try
            {
                var path = Path.GetDirectoryName(FilePath);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                File.WriteAllText(FilePath, JsonConvert.SerializeObject(_states, SerializerSettings));
            }
            catch (Exception e)
                when(e is IOException || e is Win32Exception)
            {
                
            }
        }

        private void LoadObject()
        {
            try
            {
                if (!File.Exists(FilePath))
                    return;
                _states = JsonConvert.DeserializeObject<Dictionary<Type, object>>(File.ReadAllText(FilePath), SerializerSettings)!;
            }
            catch (Exception)
            {
                _states = new Dictionary<Type, object>();
            }

            if(_states == null)
                _states = new Dictionary<Type, object>();
        }
    }
}