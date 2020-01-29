using EcsRx.Systems;
using Ninject.Modules;

namespace ImageViewerV3.Ecs.Systems
{
    public sealed class SystemModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ISystem>().To<OperationStartSystem>();
        }
    }
}