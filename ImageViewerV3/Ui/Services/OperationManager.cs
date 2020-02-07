using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Components;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui.Services
{
    public sealed class OperationManager : EcsConnector
    {
        //private class OperationMsgStade : ComputedFromGroup<string>
        //{
        //    private static readonly IGroup OpGroup = new Group(typeof(OperationComponent), typeof(OperationRunningComponent));

        //    public OperationMsgStade(IEntityCollectionManager manager)
        //        : base(manager.GetObservableGroup(OpGroup, Collections.Gui))
        //    { }

        //    public override IObservable<bool> RefreshWhen() 
        //        => InternalObservableGroup.OnEntityAdded.Merge(InternalObservableGroup.OnEntityRemoved).Select(_ => true);

        //    public override string Transform(IObservableGroup observableGroup)
        //        => observableGroup
        //               .Where(e => e.HasComponent<OperationRunningComponent>())
        //               .Select(e => e.GetComponent<OperationComponent>())
        //               .FirstOrDefault()?.Message ?? string.Empty;
        //}

        //private class OperationStade : ComputedFromGroup<bool>
        //{
        //    private static readonly IGroup OpGroup = new Group(new []{typeof(OperationComponent)});

        //    public OperationStade(IEntityCollectionManager manager) 
        //        : base(manager.GetObservableGroup(OpGroup, Collections.Gui))
        //    {
        //    }

        //    public override IObservable<bool> RefreshWhen()
        //    {
        //        return InternalObservableGroup.OnEntityAdded.Merge(InternalObservableGroup.OnEntityRemoved).Select(_ => true);
        //    }

        //    public override bool Transform(IObservableGroup observableGroup) 
        //        => observableGroup.Any(e => e.HasComponent<OperationComponent>());
        //}

        public OperationManager(IListManager listManager, IEventSystem eventSystem)
            : base(listManager, eventSystem)
        {
            var operationCache = listManager.GetList<OperationComponent>();

            DisposeThis(    
                DisposeThis(operationCache)
                    .Connect()
                    .Transform(c => new OperationEntry(c.Message, c.OpsId, c.Running, c.Sheduled))
                    .ObserveOnDispatcher()
                    .Bind(OperationCollection)
                    .DisposeMany()
                    .OnItemAdded(oe =>
                    {
                        oe.CompositeDisposable.Add(oe.IsRunning.Where(e => e).Subscribe(b =>
                        {
                            _isRunning.Value = true;
                            _message.Value = oe.ToString();
                        }));
                    })
                    .Subscribe());sdg
        }

        private readonly ReactiveProperty<bool> _isRunning = new ReactiveProperty<bool>();
        public bool IsRunning => _isRunning.Value;

        private readonly ReactiveProperty<string> _message = new ReactiveProperty<string>();
        public string Message => _message.Value;

        public IObservableCollection<OperationEntry> OperationCollection { get; } = new ObservableCollectionExtended<OperationEntry>();
    }
}