using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using ImageViewerV3.Ecs.Components.Data;

namespace ImageViewerV3.Ecs.Blueprints.Data
{
    public sealed class GeneralBlueprint : IBlueprint
    {
        private readonly string _name;
        private readonly string _value;

        public GeneralBlueprint(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public void Apply(IEntity entity)
        {
            entity.AddComponent(new TypeComponent("general"));
            entity.AddComponent(new DataComponent(_name, _value));
        }
    }
}