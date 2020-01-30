using EcsRx.Blueprints;
using EcsRx.Entities;
using EcsRx.Extensions;
using ImageViewerV3.Ecs.Components.Data;

namespace ImageViewerV3.Ecs.Blueprints.Data
{
    public sealed class FavoriteBlueprint : IBlueprint
    {
        private readonly string _name;
        private readonly string _value;

        public FavoriteBlueprint(string name, string value)
        {
            _name = name;
            _value = value;
        }

        public void Apply(IEntity entity)
        {
            entity.AddComponent(new TypeComponent("favorite"));
            entity.AddComponent<DataFavoriteComponent>();
            entity.AddComponent(new DataComponent(_name, _value));
        }
    }
}