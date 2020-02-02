using ImageViewerV3.Ecs;

namespace ImageViewerV3.Ui.Services
{
    public sealed class ImageManager
    {
        private readonly IFolderConfiguration _folderConfiguration;

        public ImageManager(IFolderConfiguration folderConfiguration) 
            => _folderConfiguration = folderConfiguration;
    }
}