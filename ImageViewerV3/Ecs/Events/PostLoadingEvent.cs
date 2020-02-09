namespace ImageViewerV3.Ecs.Events
{
    public sealed class PostLoadingEvent
    {
        public string Path { get; }

        public PostLoadingEvent(string path) => Path = path;
    }
}