using System;
using DynamicData.Kernel;
using ImageViewerV3.Ecs.Components;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Data.Impl
{
    public sealed class ImageIndexer : IImageIndexer, IDisposable
    {
        private readonly ChangeSetToCache<ImageComponent, int> _cache;

        public ImageIndexer(IListManager entityCollectionManager) 
            => _cache = new ChangeSetToCache<ImageComponent, int>(ic => ic.Index, entityCollectionManager.GetList<ImageComponent>().Connect());

        public Optional<ImageComponent> GetEntity(int index) 
            => _cache.Lookup(index);

        public void Dispose() 
            => _cache.Dispose();
    }
}