using System.IO;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Components
{
    public sealed class ImageComponent : IEntity
    {
        public string FilePath { get; } = string.Empty;

        public string Name { get; } = string.Empty;

        public int Index { get; }

        public ReactiveProperty<bool> IsFavorite { get; } = new ReactiveProperty<bool>();
        
        public ImageComponent(string filePath, int index)
        {
            FilePath = filePath;
            Index = index;
            Name = Path.GetFileName(filePath);
        }

        public override string ToString() => Name;
    }
}