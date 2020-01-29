using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using EcsRx.Collections;
using EcsRx.Infrastructure;
using EcsRx.Infrastructure.Dependencies;
using EcsRx.Infrastructure.Extensions;
using EcsRx.Infrastructure.Ninject;
using EcsRx.Plugins.Computeds;
using EcsRx.Plugins.ReactiveSystems;
using EcsRx.Plugins.Views;
using Ninject;
using Syncfusion.Licensing;

namespace ImageViewerV3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private class EcsApp : EcsRxApplication
        {
            public EcsApp(IKernel kernel) 
                => Container = new NinjectDependencyContainer(kernel);

            protected override void LoadPlugins()
            {
                RegisterPlugin(new ComputedsPlugin());
                RegisterPlugin(new ReactiveSystemsPlugin());
                RegisterPlugin(new ViewsPlugin());
            }

            protected override void BindSystems()
            {
            }

            protected override void ApplicationStarted() 
                => Container.Bind<IEntityCollection>(builder => builder.ToInstance(EntityCollectionManager.CreateCollection(1)));

            public override IDependencyContainer Container { get; }
        }

        public static readonly IKernel Kernel = new StandardKernel();
        private readonly EcsApp _app = new EcsApp(Kernel);

        public App() 
            => SyncfusionLicenseProvider.RegisterLicense("MjAzMTk2QDMxMzcyZTM0MmUzMEtJdUFuSDRMQ1RTSTczc2d6NkFaSDU5M2k0Q1BQNWxlVytzZDBDMCtiUWc9");

        protected override void OnStartup(StartupEventArgs e)
        {
            _app.StartApplication();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _app.StopApplication();
            base.OnExit(e);
        }
    }
}
