﻿using System;

namespace ImageViewerV3.Data
{
    public interface IAppState
    {
        public TType Get<TType>()
            where TType : new();

        public void Set<TType>(Action<TType> updater)
            where TType : new();
    }
}