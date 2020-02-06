using ImageViewerV3.Ui.Services;
using Ninject.Modules;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui
{
    public sealed class UiModule : DIModule
    {
        public override void Load()
        {
            Bind<MainWindowConnector>().ToSelf();
            Bind<OperationManager>().ToSelf();
            Bind<FilesManager>().ToSelf();
            Bind<ImageManager>().ToSelf();
        }
    }
}