using System;
using System.Threading.Tasks;

namespace ImageViewerV3.Ecs.Events
{
    public sealed class StartOperationEvent
    {
        public string Name { get; }

        public Func<object, Task> ToDo { get; }

        public object? Data { get; }

        public StartOperationEvent(string name, Func<object, Task> @do, object? data = null)
        {
            Name = name;
            ToDo = @do;
            Data = data;
        }
    }
}