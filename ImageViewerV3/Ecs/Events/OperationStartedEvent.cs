namespace ImageViewerV3.Ecs.Events
{
    public class OperationStartedEvent
    {
        public int Id { get; }

        public OperationStartedEvent(int id) => Id = id;
    }
}