using ImageViewerV3.Ui.Services;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui
{
    public sealed class UiModule : DIModule
    {
        public override void Load()
        {
            this.AddTransient<MainWindowConnector>();
            this.AddTransient<OperationManager>();
            this.AddTransient<FilesManager>();
            this.AddTransient<ImageManager>();
        }
    }
}