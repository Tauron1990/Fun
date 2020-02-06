using DynamicData.Kernel;
using Reactive.Bindings;

namespace ImageViewerV3.Ecs
{
    public interface IFolderConfiguration
    {
        ReactiveProperty<int> CurrentIndex { get; }
    }
}