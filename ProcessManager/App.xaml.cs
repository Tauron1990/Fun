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
        public const string SyncfusionKey = "MTc0Mjk2QDMxMzcyZTMzMmUzMElNUnVpcGhkMFhTMThkRzcvM2hSMENDc2c2YURtQS95bXhJSzVXaDduUEE9";

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
