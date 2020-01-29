using System.Collections.Generic;
using System.IO;

namespace ImageViewerV3.Ui.Libs.Gif.Decoding
{
    internal abstract class GifExtension : GifBlock
    {
        internal const int ExtensionIntroducer = 0x21;

        internal static GifExtension ReadExtension(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            // Note: at this point, the Extension Introducer (0x21) has already been read

            var label = stream.ReadByte();
            if (label < 0)
                throw GifHelpers.UnexpectedEndOfStreamException();
            return label switch
            {
                GifGraphicControlExtension.ExtensionLabel => (GifExtension) GifGraphicControlExtension.ReadGraphicsControl(stream),
                GifCommentExtension.ExtensionLabel => GifCommentExtension.ReadComment(stream),
                GifPlainTextExtension.ExtensionLabel => GifPlainTextExtension.ReadPlainText(stream, controlExtensions, metadataOnly),
                GifApplicationExtension.ExtensionLabel => GifApplicationExtension.ReadApplication(stream),
                _ => throw GifHelpers.UnknownExtensionTypeException(label)
            };
        }
    }
}
