using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using ImageViewerV3.Ecs.Components.Image;

namespace ImageViewerV3.Ecs.Blueprints.Image
{
    public sealed class ImageBlueprint : IBlueprint
    {
        private readonly string _path;

        public ImageBlueprint(string path) => _path = path;

        public void Apply(IEntity entity)
        {
            entity.AddComponent(new ImageComponent(_path));
        }
    }
}