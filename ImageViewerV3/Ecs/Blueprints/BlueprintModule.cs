using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Blueprints.Data;
using Ninject.Modules;

namespace ImageViewerV3.Ecs.Blueprints
{
    public sealed class BlueprintModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IBlueprintDescriptor>().To<GeneralBlueprintDescriptor>();
            Bind<IBlueprintDescriptor>().To<PageBlueprintDescriptor>();
            Bind<IBlueprintDescriptor>().To<FavoriteBlueprintDescriptor>();
        }
    }
}