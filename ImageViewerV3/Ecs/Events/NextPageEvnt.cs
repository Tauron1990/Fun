namespace ImageViewerV3.Ecs.Events
{
    public sealed class NextPageEvnt
    {
        public bool GoBack { get; }

        public NextPageEvnt(bool goBack) => GoBack = goBack;
    }
}