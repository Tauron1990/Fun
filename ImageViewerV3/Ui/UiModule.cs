using ImageViewerV3.Ui.Services;
using Ninject.Modules;

namespace ImageViewerV3.Ui
{
    public sealed class UiModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MainWindowConnector>().ToSelf();
            Bind<OperationManager>().ToSelf();
        }
    }
}