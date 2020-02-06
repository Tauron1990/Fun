using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Ecs.Blueprints
{
    public sealed class GeneralSettingsDescriptor : ISettingsDescriptor
    {
        public string Category => "general";
        public DataComponent Create(string name, string value) 
            => new DataComponent(name, value, Category);
    }
}