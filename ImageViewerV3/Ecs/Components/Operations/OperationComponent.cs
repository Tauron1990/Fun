using System;
using System.Threading.Tasks;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace ImageViewerV3.Ecs.Components.Operations
{
    public sealed class OperationComponent : IComponent
    {
        public string Message { get; }

        public ReactiveProperty<bool> Sheduled { get; } = new ReactiveProperty<bool>(false);

        public Func<Task> Task { get; }

        public OperationComponent(string message, Func<Task> task)
        {
            Message = message;
            Task = task;
        }
    }
}