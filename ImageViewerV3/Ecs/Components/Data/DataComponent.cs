using System;
using EcsRx.Components;
using EcsRx.ReactiveData;

namespace ImageViewerV3.Ecs.Components.Data
{
    public sealed class DataComponent : IComponent
    {
        public string Name { get; } = String.Empty;

        public ReactiveProperty<string> ReactiveValue { get; } 

        public DataComponent(string name, string value)
        {
            Name = name;
            ReactiveValue = new ReactiveProperty<string>(value);
        }

        public DataComponent()
        {
            ReactiveValue = new ReactiveProperty<string>();
        }
    }
}