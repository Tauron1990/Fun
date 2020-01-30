using System;
using System.Reactive.Linq;
using EcsRx.Attributes;
using EcsRx.Entities;
using EcsRx.Groups;
using EcsRx.Extensions;
using EcsRx.Plugins.ReactiveSystems.Systems;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Components.Data;

namespace ImageViewerV3.Ecs.Systems.Data
{
    [CollectionAffinity(Collections.Data)]
    public sealed class DataSaveSystem : IReactToEntitySystem
    {
        public IGroup Group { get; } = new Group(typeof(DataComponent));

        private readonly IDataSerializer _dataSerializer;

        public DataSaveSystem(IDataSerializer dataSerializer) 
            => _dataSerializer = dataSerializer;

        public IObservable<IEntity> ReactToEntity(IEntity entity) 
            => entity.GetComponent<DataComponent>().ReactiveValue.DistinctUntilChanged().Select(v => entity);

        public void Process(IEntity entity) 
            => _dataSerializer.Save();
    }
}