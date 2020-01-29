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

        private byte R { get; }
        private byte G { get; }
        private byte B { get; }

        public override string ToString()
        {
            return $"#{R:x2}{G:x2}{B:x2}";
        }
    }
}
