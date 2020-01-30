using System;
using EcsRx.Blueprints;

namespace ImageViewerV3.Data
{
    public interface IBlueprintDescriptor
    {
        string Name { get; }

        IBlueprint Create(string name, string value);
    }
}