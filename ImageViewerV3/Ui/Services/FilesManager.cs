using System;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui.Services
{
    public sealed class FilesManager : EcsConnector
    {
        public FilesManager(IListManager listManager, IEventSystem eventSystem)
            : base(listManager, eventSystem)
        {
            DisposeThis(eventSystem
                           .Receive<PrepareLoadEvent>()
                           .Subscribe(_ => Filter = string.Empty));


            var imageSource = listManager.GetList<ImageComponent>();

            DisposeThis(imageSource
                           .Connect()
                           .ObserveOnDispatcher()
                           .Bind(Files)
                           .Subscribe());
            
            _filter = Track(new ReactiveProperty<string>(), nameof(Filter));

            DisposeThis(imageSource
                           .Connect()
                           .Filter(FilterAction)
                           .ObserveOnDispatcher()
                           .Bind(Filtered)
                           .Subscribe());

            DisposeThis(imageSource
                           .Connect()
                           .Where(c => c.IsFavorite.Value)
                           .Filter(FilterAction)
                           .ObserveOnDispatcher()
                           .Bind(Favorites)
                           .Subscribe());
        }

        public IObservableCollection<ImageComponent> Files { get; } = new ObservableCollectionExtended<ImageComponent>();

        public IObservableCollection<ImageComponent> Filtered { get; } = new ObservableCollectionExtended<ImageComponent>();

        private readonly ReactiveProperty<string> _filter;

        public IObservable<string> FilterObservable => _filter.AsObservable();

        public string Filter
        {
            get => _filter.Value;
            set => _filter.Value = value;
        }

        private bool FilterAction(ImageComponent component) => string.IsNullOrWhiteSpace(Filter) || component.Name.Contains(Filter);

        public IObservableCollection<ImageComponent> Favorites { get; } = new ObservableCollectionExtended<ImageComponent>();
    }
}