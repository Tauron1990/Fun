using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ImageViewerV3.Ui.Libs.Gif.Decoding
{
    internal class GifFrame : GifBlock
    {
        internal const int ImageSeparator = 0x2C;

        public GifImageDescriptor? Descriptor { get; private set; }
        public GifColor[]? LocalColorTable { get; set; }
        public IList<GifExtension>? Extensions { get; private set; }
        public GifImageData? ImageData { get; set; }

        private GifFrame()
        {
        }

        internal override GifBlockKind Kind => GifBlockKind.GraphicRendering;

        internal static GifFrame ReadFrame(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            var frame = new GifFrame();

            frame.Read(stream, controlExtensions, metadataOnly);

            return frame;
        }

        private void Read(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            // Note: at this point, the Image Separator (0x2C) has already been read

            Descriptor = GifImageDescriptor.ReadImageDescriptor(stream);
            if (Descriptor.HasLocalColorTable) 
                LocalColorTable = GifHelpers.ReadColorTable(stream, Descriptor.LocalColorTableSize);
            ImageData = GifImageData.ReadImageData(stream, metadataOnly);
            Extensions = controlExtensions.ToList().AsReadOnly();
        }
    }
}
