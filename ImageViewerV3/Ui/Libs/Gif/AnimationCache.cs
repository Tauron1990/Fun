using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using JetBrains.Annotations;

namespace ImageViewerV3.Ui.Libs.Gif
{
    internal static class AnimationCache
    {
        private class CacheKey
        {
            private readonly ImageSource _source;
            private readonly RepeatBehavior _repeatBehavior;

            public CacheKey(ImageSource source, RepeatBehavior repeatBehavior)
            {
                _source = source;
                _repeatBehavior = repeatBehavior;
            }

            private bool Equals(CacheKey other)
            {
                return ImageEquals(_source, other._source)
                    && Equals(_repeatBehavior, other._repeatBehavior);
            }

            public override bool Equals(object? obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                return obj.GetType() == this.GetType() && Equals((CacheKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (ImageGetHashCode(_source) * 397) ^ _repeatBehavior.GetHashCode();
                }
            }

            private static int ImageGetHashCode(ImageSource image)
            {
                if (image == null) return 0;
                var uri = GetUri(image);
                return uri != null ? uri.GetHashCode() : 0;
            }

            private static bool ImageEquals(ImageSource x, ImageSource y)
            {
                if (Equals(x, y))
                    return true;
                if ((x == null) != (y == null))
                    return false;
                // They can't both be null or Equals would have returned true
                // and if any is null, the previous would have detected it
                // ReSharper disable PossibleNullReferenceException
                if (x!.GetType() != y!.GetType())
                    return false;
                // ReSharper restore PossibleNullReferenceException
                var xUri = GetUri(x);
                var yUri = GetUri(y);
                return xUri != null && xUri == yUri;
            }

            private static Uri? GetUri(ImageSource image)
            {
                switch (image)
                {
                    case BitmapImage bmp when bmp.UriSource != null:
                    {
                        if (bmp.UriSource.IsAbsoluteUri)
                            return bmp.UriSource;
                        if (bmp.BaseUri != null)
                            return new Uri(bmp.BaseUri, bmp.UriSource);
                        break;
                    }
                    case BitmapFrame frame:
                    {
                        var s = frame.ToString();
                        if (s != frame.GetType().FullName)
                        {
                            if (Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out var fUri))
                            {
                                if (fUri.IsAbsoluteUri)
                                    return fUri;
                                if (frame.BaseUri != null)
                                    return new Uri(frame.BaseUri, fUri);
                            }
                        }

                        break;
                    }
                }

                return null;
            }
        }

        private static readonly Dictionary<CacheKey, ObjectAnimationUsingKeyFrames> AnimationCacheDic = new Dictionary<CacheKey, ObjectAnimationUsingKeyFrames>();
        private static readonly Dictionary<CacheKey, int> ReferenceCount = new Dictionary<CacheKey, int>();

        public static void IncrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
        {
            var cacheKey = new CacheKey(source, repeatBehavior);
            ReferenceCount.TryGetValue(cacheKey, out var count);
            count++;
            ReferenceCount[cacheKey] = count;
        }

        public static void DecrementReferenceCount(ImageSource source, RepeatBehavior repeatBehavior)
        {
            var cacheKey = new CacheKey(source, repeatBehavior);
            ReferenceCount.TryGetValue(cacheKey, out var count);
            if (count > 0)
            {
                count--;
                ReferenceCount[cacheKey] = count;
            }

            if (count != 0) return;
            AnimationCacheDic.Remove(cacheKey);
            ReferenceCount.Remove(cacheKey);
        }

        public static void AddAnimation(ImageSource source, RepeatBehavior repeatBehavior, ObjectAnimationUsingKeyFrames animation)
        {
            var key = new CacheKey(source, repeatBehavior);
            AnimationCacheDic[key] = animation;
        }

        [PublicAPI]
        public static void RemoveAnimation(ImageSource source, RepeatBehavior repeatBehavior, ObjectAnimationUsingKeyFrames animation)
        {
            var key = new CacheKey(source, repeatBehavior);
            AnimationCacheDic.Remove(key);
        }

        public static ObjectAnimationUsingKeyFrames? GetAnimation(ImageSource source, RepeatBehavior repeatBehavior)
        {
            var key = new CacheKey(source, repeatBehavior);
            AnimationCacheDic.TryGetValue(key, out var animation);
            return animation;
        }
    }
}