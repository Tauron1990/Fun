using System;

namespace ImageViewerV3.Ecs.Events
{
    public class OperationFinishtEvent
    {
        public int Id { get; }

        public OperationFinishtEvent(int id) => Id = id;
    }
}