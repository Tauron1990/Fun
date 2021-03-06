﻿using System;
using System.Windows;
using System.Windows.Input;
using ImageViewerV3.Ui;
using Microsoft.Extensions.DependencyInjection;

namespace ImageViewerV3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<MainWindowConnector>();
        }

        private void MainWindow_OnClosed(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e) 
            => ((MainWindowConnector)DataContext).OnKeyDowm(e);
    }
}
