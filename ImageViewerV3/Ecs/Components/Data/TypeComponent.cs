using System;
using EcsRx.Components;

namespace ImageViewerV3.Ecs.Components.Data
{
    public sealed class TypeComponent : IComponent
    {
        public TypeComponent()
        {
        }

        public TypeComponent(string name)
        {
            Name = name;
        }

        public string? Name { get; }
    }
}