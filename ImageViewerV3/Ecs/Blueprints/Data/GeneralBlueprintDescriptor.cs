using EcsRx.Blueprints;
using ImageViewerV3.Data;

namespace ImageViewerV3.Ecs.Blueprints.Data
{
    public sealed class GeneralBlueprintDescriptor : IBlueprintDescriptor
    {
        public string Name => "general";
        public IBlueprint Create(string name, string value) 
            => new GeneralBlueprint(name, value);
    }
}