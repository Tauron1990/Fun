namespace ImageViewerV3.Ecs.Events
{
    public class LoadImagesEvent
    {
        public LoadImagesEvent(string path)
        {
            Path = path;
        }

        public string Path { get; }
    }
}