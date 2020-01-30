using System.IO;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Plugins.ReactiveSystems.Custom;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Events;

namespace ImageViewerV3.Ecs.Systems.Loading
{
    public sealed class LoadDataSystem : EventReactionSystem<LoadDataEvent>
    {
        private readonly IDataSerializer _dataSerializer;
        private readonly IEntityCollection _collection;

        public LoadDataSystem(IEventSystem eventSystem, IEntityCollectionManager entityCollectionManager, IDataSerializer dataSerializer) : base(eventSystem)
        {
            _dataSerializer = dataSerializer;
            _collection = entityCollectionManager.GetCollection(Collections.Data);
        }

        public override void EventTriggered(LoadDataEvent eventData) 
            => _dataSerializer.LoadFrom(eventData.Path, _collection);
    }
}