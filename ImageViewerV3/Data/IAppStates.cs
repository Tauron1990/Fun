using System;

namespace ImageViewerV3.Data
{
    public interface IAppStates
    {
        public TType Get<TType>()
            where TType : new();

        public void Set<TType>(Action<TType> updater)
            where TType : new();
    }
}