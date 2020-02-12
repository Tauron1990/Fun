using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using System.Windows;
using DynamicData.Kernel;
using ImageViewerV3.Core;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui.Services
{
    public sealed class ImageManager : EcsConnector
    {
        private readonly IFolderConfiguration _folderConfiguration;
        private readonly IImageIndexer _imageIndexer;
        private readonly IImageControlFactory _imageControlFactory;

        private readonly ReactiveProperty<object> _imageContent;

        private readonly Subject<Unit> _displayTrigger = new Subject<Unit>();

        private int _max;
        private bool _ready;
        private ImageComponent? _currentImage;

        public object ImageContent => _imageContent.Value;

        public ImageComponent? CurrentImage
        {
            get => _currentImage;
            set
            {
                if (Equals(value, _currentImage)) return;
                _currentImage = value;
                OnPropertyChanged();
            }
        }

        public int CurrentIndex { get; private set; }

        public ImageManager(IListManager collectionManager, IEventSystem eventSystem, IFolderConfiguration folderConfiguration, IImageIndexer imageIndexer,
                            IImageControlFactory imageControlFactory)
            :base(collectionManager, eventSystem)
        {
            _folderConfiguration = folderConfiguration;
            _imageIndexer = imageIndexer;
            _imageControlFactory = imageControlFactory;

            ReactOn<PrepareLoadEvent>(_ => _ready = false);
            ReactOn<PostLoadingEvent>(InitNewLocation);
            ReactOn<NextPageEvnt>(NextImage);

            DisposeThis(_displayTrigger
                           .Select(_ => CurrentIndex)
                           .Where(_ => _ready)
                           .Select(imageIndexer.GetEntity)
                           .Subscribe(NextImage));



            _imageContent = Track(_imageControlFactory.Output, nameof(ImageContent));

            DisposeThis(_displayTrigger);
        }

        private void NextImage(Optional<ImageComponent> img)
        {
            if (img.HasValue)
            {
                _imageControlFactory.Input.OnNext(img.Value);
                _folderConfiguration.CurrentIndex.Value = img.Value.Index - _imageIndexer.Deleted;
                CurrentImage = img.Value;
            }
            else
            {
                if (_folderConfiguration.CurrentIndex.Value == 0)
                {
                    MessageBox.Show(Application.Current.MainWindow ?? throw new InvalidOperationException(), "Fehler Bild konnte nicht geladen werden");
                    return;
                }

                CurrentIndex = 0;
                Task.Run(() => _displayTrigger.OnNext(Unit.Default));
            }
        }

        private void InitNewLocation(PostLoadingEvent @event)
        {
            _ready = true;
            CurrentIndex = _folderConfiguration.CurrentIndex.Value;
            _max = _imageIndexer.Last;

            _displayTrigger.OnNext(Unit.Default);
        }

        private void NextImage(NextPageEvnt nextPage)
        {
            while (true)
            {
                if (nextPage.GoBack)
                {
                    if (CurrentIndex == 0)
                        CurrentIndex = _max;
                    else
                        CurrentIndex--;
                }
                else
                {
                    if (CurrentIndex == _max)
                        CurrentIndex = 0;
                    else
                        CurrentIndex++;
                }

                if (_imageIndexer.IsDeleted(CurrentIndex))
                    continue;
                
                _displayTrigger.OnNext(Unit.Default);
                OnPropertyChanged(nameof(CurrentIndex));

                break;
            }
        }
    }
}