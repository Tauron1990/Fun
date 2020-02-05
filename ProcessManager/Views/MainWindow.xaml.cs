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
using Catel.Windows;
using ProcessManager.ViewModels;
using Tauron.Application.Wpf;

namespace ProcessManager.Views
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    [Control(typeof(MainWindowViewModel))]
    public partial class MainWindow
    {
        public MainWindow(MainWindowViewModel model)
            : base(model)
        {
            InitializeComponent();
        }

        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
