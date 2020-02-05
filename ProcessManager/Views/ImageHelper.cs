using System.Drawing;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Path = Catel.IO.Path;

namespace ProcessManager.Views
{
    public static class ImageHelper
    {
        private static readonly string MainIconImagePath = Path.GetFullPath("Resources\\4178928-512.png", Directory.GetCurrentDirectory());
        private static readonly string MainIconIcoPath = Path.GetFullPath("Resources\\4178928_512_xlt_icon.ico", Directory.GetCurrentDirectory());

        private static Stream OpenFile(string path)
            => File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);

        public static ImageSource MainIcon => BitmapFrame.Create(OpenFile(MainIconImagePath));

        public static Icon MainIconDrawing => new Icon(OpenFile(MainIconIcoPath));
    }
}