using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using JetBrains.Annotations;
using ProcessManager.Services;

namespace ProcessManager.Views
{
    /// <summary>
    /// Interaktionslogik für SplashScreenWindow.xaml
    /// </summary>
    public partial class SplashScreenWindow : Window
    {
        private IAppService? _appService;

        public SplashScreenWindow() 
            => InitializeComponent();

        public void Init(IAppService appService) 
            => _appService = appService;

        private void Shutdown_OnClick(object sender, RoutedEventArgs e) 
            => _appService?.Shutdown();
    }
}
