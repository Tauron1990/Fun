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
using EcsRx.Plugins.ReactiveSystems.Extensions;
using EcsRx.Plugins.Views;
using EcsRx.Plugins.Views.Extensions;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs;
using ImageViewerV3.Ecs.Blueprints;
using ImageViewerV3.Ecs.Systems;
using ImageViewerV3.Ui;
using Ninject;
using Syncfusion.Licensing;

namespace ImageViewerV3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
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
                EntityCollectionManager.CreateCollection(Collections.Data);
                EntityCollectionManager.CreateCollection(Collections.Gui);
                EntityCollectionManager.CreateCollection(Collections.Images);
            }

            protected override void StartSystems() 
                => this.StartAllBoundViewSystems();

            

            protected override void ApplicationStarted()
            {

            }

            public override IDependencyContainer Container { get; }
        }

        public static readonly IKernel Kernel = new StandardKernel(
            new SystemModule(), 
            new UiModule(), 
            new DataModule(),
            new BlueprintModule());
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
