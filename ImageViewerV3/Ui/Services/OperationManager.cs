﻿using System;
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
using ImageViewerV3.Ecs.Components.Operations;

namespace ImageViewerV3.Ui.Services
{
    public sealed class OperationManager : EcsConnector
    {
        private class OperationMsgStade : ComputedFromGroup<string>
        {
            private static readonly IGroup OpGroup = new Group(typeof(OperationComponent), typeof(OperationRunningComponent));

            public OperationMsgStade(IEntityCollectionManager manager)
                : base(manager.GetObservableGroup(OpGroup, Collections.Gui))
            { }

            public override IObservable<bool> RefreshWhen() 
                => InternalObservableGroup.OnEntityAdded.Merge(InternalObservableGroup.OnEntityRemoved).Select(_ => true);

            public override string Transform(IObservableGroup observableGroup)
                => observableGroup
                       .Where(e => e.HasComponent<OperationRunningComponent>())
                       .Select(e => e.GetComponent<OperationComponent>())
                       .FirstOrDefault()?.Message ?? string.Empty;
        }

        private class OperationStade : ComputedFromGroup<bool>
        {
            private static readonly IGroup OpGroup = new Group(new []{typeof(OperationComponent)});

            public OperationStade(IEntityCollectionManager manager) 
                : base(manager.GetObservableGroup(OpGroup, Collections.Gui))
            {
            }

            public override IObservable<bool> RefreshWhen()
            {
                return InternalObservableGroup.OnEntityAdded.Merge(InternalObservableGroup.OnEntityRemoved).Select(_ => true);
            }

            public override bool Transform(IObservableGroup observableGroup) 
                => observableGroup.Any(e => e.HasComponent<OperationComponent>());
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