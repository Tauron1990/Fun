using ImageViewerV3.Ecs.Components;

namespace ImageViewerV3.Ecs.Events
{
    public class DeleteEvent
    {
        public int Index { get; }

        public DeleteEvent(int index) => Index = index;
    }
}