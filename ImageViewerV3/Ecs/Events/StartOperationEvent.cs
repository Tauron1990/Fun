using System;
using System.Threading.Tasks;

namespace ImageViewerV3.Ecs.Events
{
    public sealed class StartOperationEvent
    {
        public string Name { get; }

        public Func<Task> ToDo { get; set; }


        public StartOperationEvent(string name, Func<Task> @do)
        {
            Name = name;
            ToDo = @do;
        }
    }
}