using EcsRx.Collections;
using EcsRx.Events;
using ImageViewerV3.Core;

namespace ImageViewerV3.Ui.Services
{
    public sealed class FilesManager : EcsConnector
    {
        public FilesManager(IEntityCollectionManager entityCollectionManager, IEventSystem eventSystem)
            : base(entityCollectionManager, eventSystem)
        {
        }
    }
}