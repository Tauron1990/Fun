namespace ImageViewerV3.Ecs.Events
{
    public sealed class BeginLoadingEvent
    {
        public string Location { get; }
        
        public BeginLoadingEvent(string location)
        {
            Location = location;
        }
    }
}