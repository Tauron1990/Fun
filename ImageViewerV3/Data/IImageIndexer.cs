using DynamicData.Kernel;
using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Data
{
    public interface IImageIndexer
    {
        Optional<ImageComponent> GetEntity(int index);
    }
}