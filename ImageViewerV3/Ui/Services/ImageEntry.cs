namespace ImageViewerV3.Ui.Services
{
    public class ImageEntry
    {
        public string Name { get; }

        public int Index { get; }

        public ImageEntry(string name, int index)
        {
            Name = name;
            Index = index;
        }
    }
}