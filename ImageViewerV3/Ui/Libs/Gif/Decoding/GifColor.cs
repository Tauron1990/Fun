namespace ImageViewerV3.Ui.Libs.Gif.Decoding
{
    internal struct GifColor
    {
        internal GifColor(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }

        public override string ToString() => $"#{R:x2}{G:x2}{B:x2}";
    }
}
