using DynamicData;
using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Data
{
    public interface IDataSerializer
    {
        void LoadFrom(string path, ISourceList<DataComponent> to);

        void Save();
    }
}