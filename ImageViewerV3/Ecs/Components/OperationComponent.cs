using System;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace ImageViewerV3.Ecs.Components
{
    public sealed class OperationComponent : IComponent
    {
        public bool IsActive { get; set; }

        public bool IsRunnging { get; set; }

        public string Message { get; set; }
    }
}