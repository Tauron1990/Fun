using System.Collections.Generic;
using System.IO;

namespace ImageViewerV3.Ui.Libs.Gif.Decoding
{
    internal abstract class GifBlock
    {
        internal static GifBlock ReadBlock(Stream stream, IEnumerable<GifExtension> controlExtensions, bool metadataOnly)
        {
            var blockId = stream.ReadByte();
            if (blockId < 0)
                throw GifHelpers.UnexpectedEndOfStreamException();
            return blockId switch
            {
                GifExtension.ExtensionIntroducer => (GifBlock) GifExtension.ReadExtension(stream, controlExtensions, metadataOnly),
                GifFrame.ImageSeparator => GifFrame.ReadFrame(stream, controlExtensions, metadataOnly),
                GifTrailer.TrailerByte => GifTrailer.ReadTrailer(),
                _ => throw GifHelpers.UnknownBlockTypeException(blockId)
            };
        }

        internal abstract GifBlockKind Kind { get; }
    }
}
