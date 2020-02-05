using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
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
            var splash = new InternalSplashScreen();
            splash.Show();

            ServiceProvider = IOCReplacer.Create(sc => { });

            MainWindow = ServiceProvider.GetRequiredService<Views.MainWindow>();
            MainWindow?.Show();
            
            splash.Hide();
        }
    }
}
