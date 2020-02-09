using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData.Kernel;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Data.Impl
{
    public sealed class ImageIndexer : EcsConnector, IImageIndexer
    {
        private readonly ChangeSetToCache<ImageComponent, int> _cache;
        private readonly HashSet<int> _removed = new HashSet<int>();

        public ImageIndexer(IListManager listManager, IEventSystem eventSystem)
            : base(listManager, eventSystem)
        {
            _cache = DisposeThis(new ChangeSetToCache<ImageComponent, int>(ic => ic.Index, listManager.GetList<ImageComponent>().Connect()));

            ReactOn<PrepareLoadEvent>(_ => _removed.Clear());
        }

        public Optional<ImageComponent> GetEntity(int index) 
            => _cache.Lookup(index);

        public int Last => _cache.Items.Max(e => e.Index);
        public void Remove(int index) 
            => _removed.Add(index);

        public bool IsDeleted(int index) 
            => _removed.Contains(index);

        public int Deleted => _removed.Count;
    }
}