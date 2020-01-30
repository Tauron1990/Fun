using System.IO;
using EcsRx.Components;

namespace ImageViewerV3.Ecs.Components.Image
{
    public sealed class ImageComponent : IComponent
    {
        public string FilePath { get; } = string.Empty;

        public string Name { get; } = string.Empty;

        public ImageComponent()
        {
            
        }

        public ImageComponent(string filePath)
        {
            FilePath = filePath;
            Name = Path.GetFileName(filePath);
        }
    }
}