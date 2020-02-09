using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DynamicData;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Systems.Loading
{
    public sealed class BeginLoadSystem : EventReactionSystem<BeginLoadingEvent>
    {
        private readonly ISourceList<ImageComponent> _imageCollection;
        private readonly ISourceList<DataComponent> _dataCollection;

        public BeginLoadSystem(IEventSystem eventSystem, IListManager listManager) : base(eventSystem)
        {
            _imageCollection = listManager.GetList<ImageComponent>();
            _dataCollection = listManager.GetList<DataComponent>();
        }

        protected override void EventTriggered(BeginLoadingEvent eventData) 
            => EventSystem.Publish(new StartOperationEvent("Bilder Laden", LoadImages, eventData));

        private Task LoadImages(object? data)
        {
            if(!(data is BeginLoadingEvent eventData))
                return Task.CompletedTask;

            if (!Directory.Exists(eventData.Location))
            {
                MessageBox.Show("Der Pfad Existiert Nicht");
                return Task.CompletedTask;
            }

            EventSystem.Publish(new PrepareLoadEvent());

            _dataCollection.Clear();
            _imageCollection.Clear();

            EventSystem.Publish(new LoadDataEvent(eventData.Location));
            EventSystem.Publish(new LoadImagesEvent(eventData.Location));
            EventSystem.Publish(new PostLoadingEvent(eventData.Location));

            return Task.CompletedTask;
        }
    }
}