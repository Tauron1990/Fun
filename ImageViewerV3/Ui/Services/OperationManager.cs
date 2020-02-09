using System;
using System.Linq;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui.Services
{
    public sealed class OperationManager : EcsConnector
    {
        private readonly object _gate = new object();

        public OperationManager(IListManager listManager, IEventSystem eventSystem)
            : base(listManager, eventSystem)
        {
            var operationCache = listManager.GetList<OperationComponent>();

            ReactOn<OperationFinishtEvent>(o => Update());
            ReactOn<OperationStartedEvent>(o => Update());

            _isRunning = Track(new ReactiveProperty<bool>(), nameof(IsRunning));

            DisposeThis(operationCache
                           .Connect()
                           .Transform(c =>
                                      {
                                          var entry = new OperationEntry(c.Message, c.OpsId);
                                          entry.CompositeDisposable.Add(entry.Finish.Subscribe(u => Update()));

                                          return entry;
                                      })
                           .ObserveOnDispatcher()
                           .Bind(OperationCollection)
                           .OnItemAdded(i => Update())
                           .OnItemRemoved(i => Update())
                           .DisposeMany()
                           .Subscribe());
        }

        private readonly ReactiveProperty<bool> _isRunning;
        public bool IsRunning => _isRunning.Value;

        private readonly ReactiveProperty<string> _message = new ReactiveProperty<string>();
        public string Message => _message.Value;

        public IObservableCollection<OperationEntry> OperationCollection { get; } = new ObservableCollectionExtended<OperationEntry>();

        private void Update()
        {
            lock (_gate) 
                _isRunning.Value = OperationCollection.Any();
        }
    }
}