using System.Drawing;
using System.Windows.Data;
using System.Windows.Media;

namespace ImageViewerV3.Core.Converter
{
    public class BoolToImageConverter : ValueConverterFactoryBase
    {
        public ImageSource? False { get; set; }

        public ImageSource? True { get; set; } 

        protected override IValueConverter Create()
        {
            return CreateCommonConverter<bool, ImageSource?>(b => b ? True : False);
        }
    }
}