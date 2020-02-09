using System;
using System.IO;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Components
{
    public sealed class ImageComponent : IEntity, IEquatable<ImageComponent>
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

        public bool Equals(ImageComponent? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return FilePath == other.FilePath;
        }

        public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is ImageComponent other && Equals(other);

        public override int GetHashCode() => FilePath.GetHashCode();

        public static bool operator ==(ImageComponent? left, ImageComponent? right) => Equals(left, right);

        public static bool operator !=(ImageComponent? left, ImageComponent? right) => !Equals(left, right);
    }
}