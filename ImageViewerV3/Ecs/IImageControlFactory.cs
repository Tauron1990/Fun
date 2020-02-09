using System;
using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Ecs
{
    public interface IImageControlFactory
    {
        IObserver<ImageComponent> Input { get; }

        IObservable<object> Output { get; }
    }
}