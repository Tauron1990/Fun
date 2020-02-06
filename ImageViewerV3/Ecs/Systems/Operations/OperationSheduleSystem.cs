using System;
using System.Threading.Tasks;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Systems;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Ecs.Systems.Operations
{
    public sealed class OperationSheduleSystem : IManualSystem, IDisposable
    {
        private class EntityObserver : IObserver<IEntity>
        {
            private readonly Action<IEntity> _next;
            private readonly Action _rest;

            public EntityObserver(Action<IEntity> next, Action rest)
            {
                _next = next;
                _rest = rest;
            }

            public void OnCompleted() 
                => _rest();

            public void OnError(Exception error) 
                => _rest();

            public void OnNext(IEntity value) 
                => _next(value);
        }

        private readonly MessageQueue<IEntity> _messageQueue = new MessageQueue<IEntity>(skipExceptions:false);

        public IGroup Group { get; } = new Group(null, new []{ typeof(OperationComponent) }, new []{typeof(OperationRunningComponent)});

        private IDisposable? _subscription;
        private readonly IEntityCollectionManager _manager;

        public OperationSheduleSystem(IEntityCollectionManager manager)
        {
            _manager = manager;
            _messageQueue.OnWork += MessageQueueOnWork;
            Task.Run(async () => await _messageQueue.Start());
        }

        private async Task MessageQueueOnWork(IEntity arg)
        {
            arg.AddComponent<OperationRunningComponent>();
            var component = arg.GetComponent<OperationComponent>(); 
            await component.Task(component.Data);
            _manager.RemoveEntity(arg);
        }

        public void StartSystem(IObservableGroup observableGroup) 
            => _subscription = observableGroup.OnEntityAdded.Subscribe(new EntityObserver(Process, () => _subscription?.Dispose()));

        private void Process(IEntity obj)
        {
            obj.GetComponent<OperationComponent>().Sheduled.Value = true;
            _messageQueue.Enqueue(obj);
        }

        public void StopSystem(IObservableGroup observableGroup) 
            => _subscription?.Dispose();

        public void Dispose()
        {
            _messageQueue.Stop();
            _messageQueue.Dispose();
        }
    }
}