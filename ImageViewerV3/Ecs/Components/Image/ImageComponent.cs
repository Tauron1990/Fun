using System.IO;
using EcsRx.Components;

namespace ImageViewerV3.Ecs.Components.Image
{
    public sealed class ImageComponent : IComponent
    {
        public string FilePath { get; } = string.Empty;

        public string Name { get; } = string.Empty;

        public int Index { get; }

        public ImageComponent()
        {
            
        }

        public ImageComponent(string filePath, int index)
        {
            FilePath = filePath;
            Index = index;
            Name = Path.GetFileName(filePath);
        }

        public override string ToString() => Name;
    }
}