using System;
using EcsRx.ReactiveData;

namespace ImageViewerV3.Ecs.Components
{
    public sealed class OperationComponent
    {
        public bool IsRunning { get; set; }

        public string Message { get; set; }
    }
}