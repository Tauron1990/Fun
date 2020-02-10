using System.Collections.Generic;
using DynamicData.Kernel;
using Reactive.Bindings;

namespace ImageViewerV3.Ecs
{
    public interface IFolderConfiguration
    {
        ReactiveProperty<int> CurrentIndex { get; }

        IReadOnlyCollection<string> Favorites { get; }

        void ToggleFavorite(string name, bool value);
    }
}