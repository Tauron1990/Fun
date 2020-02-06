using ImageViewerV3.Data;
using Microsoft.Extensions.DependencyInjection;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Blueprints
{
    public sealed class BlueprintModule : DIModule
    {
        public override void Load()
        {
            this.AddTransient<ISettingsDescriptor, GeneralSettingsDescriptor>();
            this.AddTransient<ISettingsDescriptor, PageSettingsDescriptor>();
            this.AddTransient<ISettingsDescriptor, FavoriteSettingsDescriptor>();
        }
    }
}