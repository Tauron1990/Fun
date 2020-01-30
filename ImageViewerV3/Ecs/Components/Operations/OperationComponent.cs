using System;
using System.Threading.Tasks;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace ImageViewerV3.Ecs.Components.Operations
{
    public sealed class OperationComponent : IComponent
    {
        private static int _opsIds;

        public string Message { get; }

        public ReactiveProperty<bool> Sheduled { get; } = new ReactiveProperty<bool>(false);

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