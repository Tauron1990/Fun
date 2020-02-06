using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Components
{
    public sealed class DataComponent : IEntity
    {
        public string Name { get; } = string.Empty;

        public ReactiveProperty<string> ReactiveValue { get; }

        public string Category { get; }

        public DataComponent(string name, string value, string category)
        {
            Name = name;
            Category = category;
            ReactiveValue = new ReactiveProperty<string>(value);
        }
    }
}