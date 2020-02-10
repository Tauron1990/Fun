namespace ImageViewerV3.Ecs.Events
{
    public sealed class ToogleFavoritesEvent
    {
        public int Index { get; }

        public ToogleFavoritesEvent(int index) => Index = index;
    }
}