using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Plugins.ReactiveSystems.Custom;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Blueprints.Image;
using ImageViewerV3.Ecs.Events;

namespace ImageViewerV3.Ecs.Systems.Loading
{
    public sealed class LoadImagesSystem : EventReactionSystem<LoadImagesEvent>
    {
        private sealed class NaturalStringComparer : IComparer<string?>
        {
            public static readonly NaturalStringComparer Comparer = new NaturalStringComparer();

            private NaturalStringComparer()
            {

            }

            public int Compare(string? a, string? b) => CompareNumeric(a, b);
        }

        private readonly IEntityCollection _collection;
        
        public LoadImagesSystem(IEventSystem eventSystem, IEntityCollectionManager manager) : base(eventSystem) 
            => _collection = manager.GetCollection(Collections.Images);

        public override void EventTriggered(LoadImagesEvent eventData)
        {
            foreach (var file in Directory.EnumerateFiles(eventData.Path)
                                            .Where(p =>
                                            {
                                                var mime = MimeTypes.GetMimeType(Path.GetFileName(p));
                                                return mime.StartsWith("image") || mime.StartsWith("video") || mime.StartsWith("audio");
                                            }).OrderBy(Path.GetFileName, NaturalStringComparer.Comparer)) 
                _collection.CreateEntity(new ImageBlueprint(file));
        }



        private static int CompareNumeric(string? s, string? other)
        {
            if (s == null || other == null || (s = s.Replace(" ", string.Empty)).Length <= 0 || (other = other.Replace(" ", string.Empty)).Length <= 0) return 0;
            int sIndex = 0, otherIndex = 0;

            while (sIndex < s.Length)
            {
                if (otherIndex >= other.Length)
                    return 1;

                if (char.IsDigit(s[sIndex]))
                {
                    if (!char.IsDigit(other[otherIndex]))
                        return -1;

                    // Compare the numbers
                    StringBuilder sBuilder = new StringBuilder(), otherBuilder = new StringBuilder();

                    while (sIndex < s.Length && char.IsDigit(s[sIndex]))
                        sBuilder.Append(s[sIndex++]);

                    while (otherIndex < other.Length && char.IsDigit(other[otherIndex]))
                        otherBuilder.Append(other[otherIndex++]);

                    long sValue, otherValue;

                    try
                    {
                        sValue = Convert.ToInt64(sBuilder.ToString());
                    }
                    catch (OverflowException) { sValue = long.MaxValue; }

                    try
                    {
                        otherValue = Convert.ToInt64(otherBuilder.ToString());
                    }
                    catch (OverflowException) { otherValue = long.MaxValue; }

                    if (sValue < otherValue)
                        return -1;
                    if (sValue > otherValue)
                        return 1;
                }
                else if (char.IsDigit(other[otherIndex]))
                    return 1;
                else
                {
                    var difference = string.Compare(s[sIndex].ToString(), other[otherIndex].ToString(), StringComparison.InvariantCultureIgnoreCase);

                    if (difference > 0)
                        return 1;
                    if (difference < 0)
                        return -1;

                    sIndex++;
                    otherIndex++;
                }
            }

            if (otherIndex < other.Length)
                return -1;

            return 0;
        }
    }
}