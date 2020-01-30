namespace ImageViewerV3.Ecs.Events
{
    public sealed class LoadDataEvent
    {
        public LoadDataEvent(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}