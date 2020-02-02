using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Plugins.ReactiveSystems.Custom;
using ImageViewerV3.Ecs.Events;

namespace ImageViewerV3.Ecs.Systems.Loading
{
    public sealed class BeginLoadSystem : EventReactionSystem<BeginLoadingEvent>
    {
        private readonly IEntityCollection _imageCollection;
        private readonly IEntityCollection _dataCollection;

        public BeginLoadSystem(IEventSystem eventSystem, IEntityCollectionManager entityCollectionManager) : base(eventSystem)
        {
            _imageCollection = entityCollectionManager.GetCollection(Collections.Images);
            _dataCollection = entityCollectionManager.GetCollection(Collections.Data);
        }

        public override void EventTriggered(BeginLoadingEvent eventData)
        {
            EventSystem.Publish(new StartOperationEvent("Bilder Laden", LoadImages, eventData));
        }

        private Task LoadImages(object data)
        {
            if(!(data is BeginLoadingEvent eventData))
                return Task.CompletedTask;

            if (!Directory.Exists(eventData.Location))
            {
                MessageBox.Show("Der Pfad Existiert Nicht");
                return Task.CompletedTask;
            }

            RemoveAllEntitys(_dataCollection);
            RemoveAllEntitys(_imageCollection);

            EventSystem.Publish(new PrepareLoadEvent());
            EventSystem.Publish(new LoadDataEvent(eventData.Location));
            EventSystem.Publish(new LoadImagesEvent(eventData.Location));
            EventSystem.Publish(new PostLoadingEvent());

            return Task.CompletedTask;
        }

        private static void RemoveAllEntitys(IEntityCollection collection)
        {
            foreach (var id in collection.Select(e => e.Id).ToArray()) 
                collection.RemoveEntity(id);
        }
    }
}