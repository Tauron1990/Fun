using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Ecs.Blueprints
{
    public sealed class FavoriteSettingsDescriptor : ISettingsDescriptor
    {
        public string Category { get; } = "Favorite";
        public DataComponent Create(string name, string value)
            => new DataComponent(name, value, Category);
    }
}