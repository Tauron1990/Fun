using System;
using System.Linq;
using System.Reactive.Linq;
using EcsRx.Collections;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.Computeds;
using EcsRx.ReactiveData;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs;
using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Ui.Services
{
    public sealed class OperationManager : EcsConnector
    {
        private class OperationMsgStade : ComputedFromGroup<string>
        {
            public OperationMsgStade(IEntityCollectionManager manager)
                : base(manager.GetObservableGroup(OperationStade.OpGroup, Collections.Gui))
            {
            }

            public override IObservable<bool> RefreshWhen()
            {
                return InternalObservableGroup.OnEntityAdded.Concat(InternalObservableGroup.OnEntityRemoved).Select(_ => true);
            }

            public override string Transform(IObservableGroup observableGroup)
                => observableGroup.Select(e => e.GetComponent<OperationComponent>()).FirstOrDefault()?.Message ?? string.Empty;
        }

        private class OperationStade : ComputedFromGroup<bool>
        {
            public static readonly IGroup OpGroup = new Group(new []{typeof(OperationComponent)});

            public OperationStade(IEntityCollectionManager manager) 
                : base(manager.GetObservableGroup(OpGroup, Collections.Gui))
            {
            }

            public override IObservable<bool> RefreshWhen()
            {
                return InternalObservableGroup.OnEntityAdded.Concat(InternalObservableGroup.OnEntityRemoved).Select(_ => true);
            }

            public override bool Transform(IObservableGroup observableGroup) 
                => observableGroup.Select(e => e.GetComponent<OperationComponent>()).Any(c => c.IsActive);
        }
        
        public OperationManager(IEntityCollectionManager entityCollectionManager)
        {
            _isRunning = Track(new OperationStade(entityCollectionManager), nameof(IsRunning));
            _message = Track(new OperationMsgStade(entityCollectionManager), nameof(Message));
        }

        private readonly ReactiveProperty<bool> _isRunning;
        public bool IsRunning => _isRunning.Value;

        private readonly ReactiveProperty<string> _message;
        public string Message => _message.Value;
    }
}