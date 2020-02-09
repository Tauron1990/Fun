using System.Windows;

namespace ImageViewerV3.Data.AppStates
{
    public sealed class AppWindowState
    {
        public Visibility ControlVisibility { get; set; } = Visibility.Visible;

        public WindowState WindowState { get; set; } = WindowState.Maximized;

        public WindowStyle WindowStyle { get; set; } = WindowStyle.SingleBorderWindow;

        public WindowStartupLocation WindowStartupLocation { get; set; } = WindowStartupLocation.CenterScreen;

        public int Height { get; set; } = 450;

        public int Width { get; set; } = 800;
    }
}