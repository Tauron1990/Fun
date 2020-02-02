using DynamicData.Kernel;
using EcsRx.Plugins.Computeds;
using EcsRx.ReactiveData;

namespace ImageViewerV3.Ecs
{
    public interface IFolderConfiguration
    {
        IComputed<Optional<SelecedImage>> SelectedImage { get; }

        ReactiveProperty<int> CurrentIndex { get; }
    }
}