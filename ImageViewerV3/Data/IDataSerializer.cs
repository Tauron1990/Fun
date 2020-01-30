using EcsRx.Collections;

namespace ImageViewerV3.Data
{
    public interface IDataSerializer
    {
        void LoadFrom(string path, IEntityCollection to);

        void Save();
    }
}