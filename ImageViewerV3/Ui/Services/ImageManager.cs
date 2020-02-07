using ImageViewerV3.Core;
using ImageViewerV3.Ecs;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui.Services
{
    public sealed class ImageManager : EcsConnector
    {
        private readonly IFolderConfiguration _folderConfiguration;

        private readonly ReactiveProperty<object> _imageContent;
        public object ImageContent => _imageContent.Value;

        public ImageManager(IListManager collectionManager, IEventSystem eventSystem, IFolderConfiguration folderConfiguration)
            :base(collectionManager, eventSystem)
        {
            _imageContent = Track(new ReactiveProperty<object>(), nameof(ImageContent));
            _folderConfiguration = folderConfiguration;
        }
    }
}