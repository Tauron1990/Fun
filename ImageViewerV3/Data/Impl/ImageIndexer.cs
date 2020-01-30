using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using EcsRx.Collections;
using EcsRx.Groups;
using ImageViewerV3.Ecs;
using ImageViewerV3.Ecs.Components.Image;

namespace ImageViewerV3.Data.Impl
{
    public sealed class ImageIndexer : IImageIndexer, IDisposable
    {
        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly List<int> _indexes = new List<int>();

        public ImageIndexer(IEntityCollectionManager entityCollectionManager)
        {
            var group = entityCollectionManager.GetObservableGroup(new Group(typeof(ImageComponent)), Collections.Images);

            _disposable.Add(group.OnEntityRemoved.Subscribe(e => _indexes.Add(e.Id)));
            _disposable.Add(group.OnEntityAdded.Subscribe(e => _indexes.Remove(e.Id)));
        }

        public int? GetEntity(int index) 
            => _indexes[index];

        public void Dispose() 
            => _disposable.Dispose();
    }
}