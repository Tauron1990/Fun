using ImageViewerV3.Data.Impl;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Data
{
    public sealed class DataModule : DIModule
    {
        public override void Load()
        {
            this.AddSingleton<IDataSerializer, DataSerializer>();
            this.AddSingleton<IImageIndexer, ImageIndexer>();
        }
    }
}