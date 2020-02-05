using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace ProcessManager.Services.Impl
{
    [ServiceDescriptor(typeof(ILayoutManager), ServiceLifetime.Singleton)]
    public sealed class LayoutManager : ILayoutManager
    {
        
    }
}