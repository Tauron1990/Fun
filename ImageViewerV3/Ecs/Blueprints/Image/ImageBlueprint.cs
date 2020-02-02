using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using ImageViewerV3.Ecs.Components.Image;

namespace ImageViewerV3.Ecs.Blueprints.Image
{
    public sealed class ImageBlueprint : IBlueprint
    {
        private readonly string _path;
        private readonly int _index;

        public ImageBlueprint(string path, int index)
        {
            _path = path;
            _index = index;
        }

        public void Apply(IEntity entity)
        {
            entity.AddComponent(new ImageComponent(_path, _index));
        }
    }
}