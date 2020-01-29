namespace ImageViewerV3.Ecs.Events
{
    public sealed class StartOperationEvent
    {
        public string Name { get; }

        public StartOperationEvent(string name) 
            => Name = name;
    }
}