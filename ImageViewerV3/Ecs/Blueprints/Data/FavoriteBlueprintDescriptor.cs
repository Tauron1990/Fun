using EcsRx.Blueprints;
using ImageViewerV3.Data;

namespace ImageViewerV3.Ecs.Blueprints.Data
{
    public sealed class FavoriteBlueprintDescriptor : IBlueprintDescriptor
    {
        public string Name { get; } = "Favorite";
        public IBlueprint Create(string name, string value)
            => new FavoriteBlueprint(name, value);
    }
}