using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Tauron.Application.Reactive
{
    [PublicAPI]
    public static class ReactiveExtensions
    {
        public static IServiceCollection AddReactiveExtensions(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IListManager, ListManager>();
            serviceCollection.AddSingleton<IEventSystem, EventSystem>();

            return serviceCollection;
        }

        public static IServiceCollection AddModules(this IServiceCollection serviceCollection, params DIModule[] modules)
        {
            foreach (var module in modules)
            {
                module.ServiceCollection = serviceCollection;
                module.Load();
            }

            return serviceCollection;
        }
    }
}