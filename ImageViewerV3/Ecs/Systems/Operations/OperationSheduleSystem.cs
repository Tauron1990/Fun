using System;
using System.Threading.Tasks;
using DynamicData;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Systems.Operations
{
    public sealed class OperationSheduleSystem : ISystem, IDisposable
    {
        private readonly IEventSystem _eventSystem;
        private readonly MessageQueue<OperationComponent> _messageQueue = new MessageQueue<OperationComponent>(skipExceptions:false);
        
        private IDisposable? _subscription;
        private ISourceList<OperationComponent> _operations = new SourceList<OperationComponent>();

        public OperationSheduleSystem(IEventSystem eventSystem)
        {
            _eventSystem = eventSystem;
            _messageQueue.OnWork += MessageQueueOnWork;
            Task.Run(async () => await _messageQueue.Start());
        }

        private async Task MessageQueueOnWork(OperationComponent arg)
        {
            _eventSystem.Publish(new OperationStartedEvent(arg.OpsId));
            await arg.Task(arg.Data);
            _operations.Remove(arg);
            _eventSystem.Publish(new OperationFinishtEvent(arg.OpsId));
        }
        
        private void Process(OperationComponent obj) 
            => _messageQueue.Enqueue(obj);

        public void Dispose()
        {
            _subscription?.Dispose();
            _messageQueue.Stop();
            _messageQueue.Dispose();
        }

        public void Init(IListManager listManager)
        {
            _operations = listManager.GetList<OperationComponent>();
            _subscription = listManager.GetList<OperationComponent>().Connect().OnItemAdded(Process).Subscribe();
        }
    }
}