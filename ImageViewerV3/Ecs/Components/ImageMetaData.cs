using System;

namespace ImageViewerV3.Ecs.Components
{
    public sealed class ImageMetaData
    {
        public string User { get; } = string.Empty;

        public int Height { get; } = 0;

        public int Width { get; } = 0;

        public int Mpixels { get; } = 0;

        public string[] Tags { get; } = Array.Empty<string>();

        public DateTime CreationTime { get; } = DateTime.MinValue;

        public int Rating { get; } = 0;

        public ImageMetaData()
        {
            
        }

        public ImageMetaData(string user, int height, int width, int mpixels, string[] tags, DateTime creationTime, int rating)
        {
            User = user;
            Height = height;
            Width = width;
            Mpixels = mpixels;
            Tags = tags;
            CreationTime = creationTime;
            Rating = rating;
        }
    }
}