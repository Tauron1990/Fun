using System;
using System.Reactive.Linq;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Components;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Systems.Data
{
    public sealed class DataSaveSystem : ReactToEntitySystem<DataComponent>
    {

        private readonly IDataSerializer _dataSerializer;

        public DataSaveSystem(IDataSerializer dataSerializer) 
            => _dataSerializer = dataSerializer;

        protected override IObservable<DataComponent> ReactTo(DataComponent entity) 
            => entity.ReactiveValue.DistinctUntilChanged().Select(s => entity);

        protected override void Process(DataComponent entity) 
            => _dataSerializer.Save();
    }
}