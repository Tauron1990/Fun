using EcsRx.Systems;
using ImageViewerV3.Ecs.Systems.Data;
using ImageViewerV3.Ecs.Systems.Image;
using ImageViewerV3.Ecs.Systems.Loading;
using ImageViewerV3.Ecs.Systems.Operations;
using Ninject.Modules;

namespace ImageViewerV3.Ecs.Systems
{
    public sealed class SystemModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISystem>().To<OperationStartSystem>();
            Bind<ISystem>().To<OperationSheduleSystem>();
            Bind<ISystem>().To<BeginLoadSystem>();
            Bind<ISystem>().To<LoadDataSystem>();
            Bind<ISystem>().To<DataSaveSystem>();
            Bind<ISystem>().To<LoadImagesSystem>();
            Bind<ISystem>().To<FavoriteTrackerSystem>();
        }
    }
}