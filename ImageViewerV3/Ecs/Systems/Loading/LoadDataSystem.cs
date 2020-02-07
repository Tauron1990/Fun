using DynamicData;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Systems.Loading
{
    public sealed class LoadDataSystem : EventReactionSystem<LoadDataEvent>
    {
        private readonly IDataSerializer _dataSerializer;
        private readonly ISourceList<DataComponent> _collection;

        public LoadDataSystem(IEventSystem eventSystem, IListManager entityCollectionManager, IDataSerializer dataSerializer) : base(eventSystem)
        {
            _dataSerializer = dataSerializer;
            _collection = entityCollectionManager.GetList<DataComponent>();
        }

        protected override void EventTriggered(LoadDataEvent eventData) 
            => _dataSerializer.LoadFrom(eventData.Path, _collection);
    }
}