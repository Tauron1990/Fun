using System;
using System.Collections.Generic;
using DynamicData;
using DynamicData.Kernel;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Groups;
using ImageViewerV3.Ecs;

namespace ImageViewerV3.Core
{
    public sealed class GroupToList : ISourceList<IEntity>
    {
        private readonly SourceList<IEntity> _source = new SourceList<IEntity>();
        private readonly IDisposable _subsription1;
        private readonly IDisposable _subsription2;

        public GroupToList(IEntityCollectionManager manager, IGroup group, params int[] collections)
        {
            var obserGroup = manager.GetObservableGroup(group, collections);
            _subsription1 = obserGroup.OnEntityAdded.Subscribe(e => _source.Add(e));
            _subsription2 = obserGroup.OnEntityRemoved.Subscribe(e => _source.Remove(e));
        }

        public void Edit(Action<IExtendedList<IEntity>> updateAction)
        {
            _source.Edit(updateAction);
        }

        public void Dispose()
        {
            _subsription1.Dispose();
            _subsription2.Dispose();
            _source.Dispose();
        }

        public IObservable<IChangeSet<IEntity>> Connect(Func<IEntity, bool>? predicate = null) => _source.Connect(predicate);

        public IObservable<IChangeSet<IEntity>> Preview(Func<IEntity, bool>? predicate = null) => _source.Preview(predicate);

        public IObservable<int> CountChanged => _source.CountChanged;

        public IEnumerable<IEntity> Items => _source.Items;

        public int Count => _source.Count;
    }

    public sealed class GroupToCache : ISourceCache<IEntity, int>
    {
        private readonly SourceCache<IEntity, int> _source = new SourceCache<IEntity, int>(e => e.Id);
        private readonly IDisposable _subsription1;
        private readonly IDisposable _subsription2;

        public GroupToCache(IEntityCollectionManager manager, IGroup group, params int[] collections)
        {
            var obserGroup = manager.GetObservableGroup(group, collections);
            _subsription1 = obserGroup.OnEntityAdded.Subscribe(e => _source.AddOrUpdate(e));
            _subsription2 = obserGroup.OnEntityRemoved.Subscribe(e => _source.Remove(e));
        }

        public void Edit(Action<ISourceUpdater<IEntity, int>> updateAction)
        {
            _source.Edit(updateAction);
        }

        public void Dispose()
        {
            _subsription1.Dispose();
            _subsription2.Dispose();
            _source.Dispose();
        }

        public IObservable<Change<IEntity, int>> Watch(int key) => _source.Watch(key);

        public IObservable<IChangeSet<IEntity, int>> Connect(Func<IEntity, bool>? predicate = null) => _source.Connect(predicate);

        public IObservable<IChangeSet<IEntity, int>> Preview(Func<IEntity, bool>? predicate = null) => _source.Preview(predicate);

        public IObservable<int> CountChanged => _source.CountChanged;

        public Optional<IEntity> Lookup(int key) => _source.Lookup(key);

        public IEnumerable<int> Keys => _source.Keys;

        public IEnumerable<IEntity> Items => _source.Items;

        public IEnumerable<KeyValuePair<int, IEntity>> KeyValues => _source.KeyValues;

        public int Count => _source.Count;
    }
}