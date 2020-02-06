using System.Windows;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs;
using ImageViewerV3.Ecs.Blueprints;
using ImageViewerV3.Ecs.Systems;
using ImageViewerV3.Ui;
using Syncfusion.Licensing;

namespace ImageViewerV3
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static readonly IKernel Kernel = new StandardKernel(
            new SystemModule(), 
            new UiModule(), 
            new DataModule(),
            new BlueprintModule(),
            new EcsModule());

        public App() 
            => SyncfusionLicenseProvider.RegisterLicense("MjA2MjcyQDMxMzcyZTM0MmUzMEtaZlplNTBLMWZrRGgyR0dDV3QrdDVIMHRiOGxNZ0QyOENINWs5cUoxY2c9");

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
