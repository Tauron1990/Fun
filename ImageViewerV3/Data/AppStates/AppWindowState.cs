using System.Windows;

namespace ImageViewerV3.Data.AppStates
{
    public sealed class AppWindowState
    {
        public Visibility ControlVisibility { get; set; } = Visibility.Visible;

        public WindowState WindowState { get; set; } = WindowState.Maximized;

        public WindowStyle WindowStyle { get; set; } = WindowStyle.SingleBorderWindow;

        public bool TopMost { get; set; } = false;

        public double Height { get; set; } = 450;

        public double Width { get; set; } = 800;
    }
}