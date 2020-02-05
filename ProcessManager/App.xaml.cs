using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using ProcessManager.Services;
using ProcessManager.Views;
using Tauron.Application.Wpf;

namespace ProcessManager
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string SyncfusionKey = "MjA2MjcyQDMxMzcyZTM0MmUzMEtaZlplNTBLMWZrRGgyR0dDV3QrdDVIMHRiOGxNZ0QyOENINWs5cUoxY2c9";

        public static IServiceProvider ServiceProvider { get; private set; }


        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SyncfusionKey);
        }
        
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            var splash = new SplashScreenWindow();
            splash.Show();

            ServiceProvider = IOCReplacer.Create(sc => { sc.AddSingleton(this); });

            var window = ServiceProvider.GetRequiredService<Views.MainWindow>();
            window?.Show();
            
            splash.Init(ServiceProvider.GetRequiredService<IAppService>());
            splash.Hide();
        }
    }
}
