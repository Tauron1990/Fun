using EcsRx.Blueprints;
using ImageViewerV3.Data;

namespace ImageViewerV3.Ecs.Blueprints.Data
{
    public sealed class PageBlueprintDescriptor : IBlueprintDescriptor
    {
        public string Name { get; } = "Paging";
        public IBlueprint Create(string name, string value) 
            => new PageBlueprint(name, value);
    }
}