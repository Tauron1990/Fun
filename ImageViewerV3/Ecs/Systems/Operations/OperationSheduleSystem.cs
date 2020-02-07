using System;
using System.Threading.Tasks;
using DynamicData;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Components;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Systems.Operations
{
    public sealed class OperationSheduleSystem : ISystem, IDisposable
    {
        private readonly MessageQueue<OperationComponent> _messageQueue = new MessageQueue<OperationComponent>(skipExceptions:false);
        
        private IDisposable? _subscription;
        private readonly ISourceList<OperationComponent> _operations = new SourceList<OperationComponent>();

        public OperationSheduleSystem()
        {
            _messageQueue.OnWork += MessageQueueOnWork;
            Task.Run(async () => await _messageQueue.Start());
        }

        private async Task MessageQueueOnWork(OperationComponent arg)
        {
            arg.Running.Value = true;
            await arg.Task(arg.Data);
            _operations.Remove(arg);
        }
        
        private void Process(OperationComponent obj)
        {
            obj.Sheduled.Value = true;
            _messageQueue.Enqueue(obj);
        }

        public void Dispose()
        {
            _subscription?.Dispose();
            _messageQueue.Stop();
            _messageQueue.Dispose();
        }

        public void Init(IListManager listManager) 
            => _subscription = listManager.GetList<OperationComponent>().Connect().OnItemAdded(Process).Subscribe();
    }
}