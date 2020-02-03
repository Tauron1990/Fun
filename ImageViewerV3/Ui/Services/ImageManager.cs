using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.ReactiveData;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs;

namespace ImageViewerV3.Ui.Services
{
    public sealed class ImageManager : EcsConnector
    {
        private readonly IFolderConfiguration _folderConfiguration;

        private ReactiveProperty<object> _imageContent;
        public object ImageContent => _imageContent.Value;

        public ImageManager(IEntityCollectionManager collectionManager, IEventSystem eventSystem, IFolderConfiguration folderConfiguration)
            :base(collectionManager, eventSystem)
        {
            _imageContent = Track(new ReactiveProperty<object>(), nameof(ImageContent));
            _folderConfiguration = folderConfiguration;
        }
    }
}