using System;
using EcsRx.ReactiveData;

namespace ImageViewerV3
{
    public sealed class SelecedImage
    {
        public string ImageName { get; }


        public string FilePath { get; }

        public bool IsFavorite { get; }

        public SelecedImage(string imageName, string filePath, bool isFavorite)
        {
            ImageName = imageName;
            FilePath = filePath;
            IsFavorite = isFavorite;
        }
    }
}