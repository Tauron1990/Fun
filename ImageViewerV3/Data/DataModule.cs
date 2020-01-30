using ImageViewerV3.Data.Impl;
using Ninject.Modules;

namespace ImageViewerV3.Data
{
    public sealed class DataModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IDataSerializer>().To<DataSerializer>().InSingletonScope();
            Bind<IImageIndexer>().To<ImageIndexer>().InSingletonScope();
        }
    }
}