using ImageViewerV3.Ecs.Services;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs
{
    public sealed class EcsModule : DIModule
    {
        public override void Load()
        {
            this.AddSingleton<IFolderConfiguration, FolderConfiguration>();
            this.AddSingleton<IImageControlFactory, ImageControlFactory>();
        }
    }
}