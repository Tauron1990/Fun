using Scrutor;

namespace ProcessManager.Services.Impl
{
    [ServiceDescriptor(typeof(IAppService))]
    public sealed class AppService : IAppService
    {
        private readonly App _app;

        public AppService(App app) 
            => _app = app;

        public void Shutdown() 
            => _app.Shutdown();
    }
}