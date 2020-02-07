using System.Windows;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs;
using ImageViewerV3.Ecs.Blueprints;
using ImageViewerV3.Ecs.Systems;
using ImageViewerV3.Ui;
using Microsoft.Extensions.DependencyInjection;
using Syncfusion.Licensing;
using Tauron.Application.Reactive;

namespace ImageViewerV3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static ServiceProvider? ServiceProvider { get; private set; }

        public App() 
            => SyncfusionLicenseProvider.RegisterLicense("MjA2MjcyQDMxMzcyZTM0MmUzMEtaZlplNTBLMWZrRGgyR0dDV3QrdDVIMHRiOGxNZ0QyOENINWs5cUoxY2c9");

        protected override void OnStartup(StartupEventArgs e)
        {
            var coll = new ServiceCollection();

            coll.AddTauronReactive();
            coll.AddModules(new SystemModule(),
                new UiModule(),
                new DataModule(),
                new BlueprintModule(),
                new EcsModule());

            ServiceProvider = coll.BuildServiceProvider();
            ServiceProvider.InitSystems();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
