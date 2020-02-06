using System;
using System.Threading.Tasks;
using Reactive.Bindings;

namespace ImageViewerV3.Ecs.Components
{
    public sealed class OperationComponent
    {
        private static int _opsIds;

        public string Message { get; }

        public ReactiveProperty<bool> Sheduled { get; } = new ReactiveProperty<bool>();

        public ReactiveProperty<bool> Running { get; } = new ReactiveProperty<bool>();

        public Func<object, Task> Task { get; }
        public object Data { get; }

        public int OpsId { get; } = ++_opsIds;

        public OperationComponent(string message, Func<object, Task> task, object data)
        {
            Message = message;
            Task = task;
            Data = data;
        }
    }
}