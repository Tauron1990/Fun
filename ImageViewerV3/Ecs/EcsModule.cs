using ImageViewerV3.Ecs.Services;
using Ninject.Modules;

namespace ImageViewerV3.Ecs
{
    public sealed class EcsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IFolderConfiguration>().To<FolderConfiguration>().InSingletonScope();
        }
    }
}