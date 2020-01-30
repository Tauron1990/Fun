using System;
using System.IO;
using System.Text;

namespace ImageViewerV3.Ui.Libs.Gif.Decoding
{
    internal static class GifHelpers
    {
        public static string ReadString(Stream stream, int length)
        {
            var bytes = new byte[length];
            stream.ReadAll(bytes, 0, length);
            return Encoding.ASCII.GetString(bytes);
        }

        public static byte[]? ReadDataBlocks(Stream stream, bool discard)
        {
            var ms = discard ? null : new MemoryStream();
            using (ms)
            {
                int len;
                while ((len = stream.ReadByte()) > 0)
                {
                    var bytes = new byte[len];
                    stream.ReadAll(bytes, 0, len);
                    ms?.Write(bytes, 0, len);
                }

                return ms?.ToArray();
            }
        }

        public static GifColor[] ReadColorTable(Stream stream, int size)
        {
            var length = 3 * size;
            var bytes = new byte[length];
            stream.ReadAll(bytes, 0, length);
            var colorTable = new GifColor[size];
            for (var i = 0; i < size; i++)
            {
                var r = bytes[3 * i];
                var g = bytes[3 * i + 1];
                var b = bytes[3 * i + 2];
                colorTable[i] = new GifColor(r, g, b);
            }
            return colorTable;
        }

        public static bool IsNetscapeExtension(GifApplicationExtension ext)
        {
            return ext.ApplicationIdentifier == "NETSCAPE"
                && Encoding.ASCII.GetString(ext.AuthenticationCode ?? Array.Empty<byte>()) == "2.0";
        }

        public static ushort GetRepeatCount(GifApplicationExtension ext)
        {
            return ext.Data?.Length >= 3 ? BitConverter.ToUInt16(ext.Data, 1) : (ushort) 1;
        }

        public static Exception UnexpectedEndOfStreamException() 
            => new GifDecoderException("Unexpected end of stream before trailer was encountered");

        public static Exception UnknownBlockTypeException(int blockId) 
            => new GifDecoderException("Unknown block type: 0x" + blockId.ToString("x2"));

        public static Exception UnknownExtensionTypeException(int extensionLabel)
            => new GifDecoderException("Unknown extension type: 0x" + extensionLabel.ToString("x2"));

        public static Exception InvalidBlockSizeException(string blockName, int expectedBlockSize, int actualBlockSize) =>
            new GifDecoderException($"Invalid block size for {blockName}. Expected {expectedBlockSize}, but was {actualBlockSize}");

        public static Exception InvalidSignatureException(string signature) 
            => new GifDecoderException("Invalid file signature: " + signature);

        public static Exception UnsupportedVersionException(string version) 
            => new GifDecoderException("Unsupported version: " + version);

        public static void ReadAll(this Stream stream, byte[] buffer, int offset, int count)
        {
            var totalRead = 0;
            while (totalRead < count) 
                totalRead += stream.Read(buffer, offset + totalRead, count - totalRead);
        }
    }
}
