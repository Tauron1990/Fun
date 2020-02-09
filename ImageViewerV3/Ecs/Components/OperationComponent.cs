using System;
using System.Threading.Tasks;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Components
{
    public sealed class OperationComponent : IEntity
    {
        private static int _opsIds;

        public string Message { get; }

        public Func<object?, Task> Task { get; }
        public object? Data { get; }

        public int OpsId { get; } = ++_opsIds;

        public OperationComponent(string message, Func<object?, Task> task, object? data)
        {
            Message = message;
            Task = task;
            Data = data;
        }
    }
}