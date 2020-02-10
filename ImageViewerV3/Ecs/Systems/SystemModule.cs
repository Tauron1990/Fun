using ImageViewerV3.Ecs.Systems.Data;
using ImageViewerV3.Ecs.Systems.Loading;
using ImageViewerV3.Ecs.Systems.Operations;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Systems
{
    public sealed class SystemModule : DIModule
    {
        public override void Load()
        {
            this.AddSingleton<ISystem, OperationStartSystem>();
            this.AddSingleton<ISystem, OperationSheduleSystem>();
            this.AddSingleton<ISystem, BeginLoadSystem>();
            this.AddSingleton<ISystem, LoadDataSystem>();
            this.AddSingleton<ISystem, DataSaveSystem>();
            this.AddSingleton<ISystem, LoadImagesSystem>();
            this.AddSingleton<ISystem, FavoritesSystem>();
        }
    }
}