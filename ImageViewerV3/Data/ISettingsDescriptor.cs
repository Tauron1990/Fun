using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Data
{
    public interface ISettingsDescriptor
    {
        string Category { get; }

        DataComponent Create(string name, string value);
    }
}