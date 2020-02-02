using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.ReactiveData;
using ImageViewerV3.Core;
using ImageViewerV3.Ecs;
using ImageViewerV3.Ecs.Components.Image;
using ImageViewerV3.Ecs.Events;

namespace ImageViewerV3.Ui.Services
{
    public sealed class FilesManager : EcsConnector
    {
        public FilesManager(IEntityCollectionManager entityCollectionManager, IEventSystem eventSystem)
            : base(entityCollectionManager, eventSystem)
        {
            DisposeThis(eventSystem
                           .Receive<PrepareLoadEvent>()
                           .Subscribe(_ => Filter = string.Empty));

            var group = new Group(typeof(ImageComponent));
            var imageSource = DisposeThis(new GroupToCache(entityCollectionManager, group, Collections.Images));

            DisposeThis(imageSource
                           .Connect()
                           .Transform(e => e.GetComponent<ImageComponent>())
                           .ObserveOnDispatcher()
                           .Bind(Files)
                           .Subscribe());
            
            _filter = Track(new ReactiveProperty<string>(), nameof(Filter));
            DisposeThis(_filter.Subscribe(s =>
                                          {
                                              if(!string.IsNullOrWhiteSpace(s))
                                                StartFilter.Execute();
                                          }));

            DisposeThis(imageSource
                           .Connect()
                           .Transform(e => e.GetComponent<ImageComponent>())
                           .Filter(Observable.Return(new Func<ImageComponent, bool>(FilterAction)), StartFilter)
                           .ObserveOnDispatcher()
                           .Bind(Filtered)
                           .Subscribe());

            DisposeThis(imageSource
                           .Connect()
                           .Filter(e => e.HasComponent<IsFavoriteComponent>())
                           .Transform(e => e.GetComponent<ImageComponent>())
                           .Filter(Observable.Return(new Func<ImageComponent, bool>(FilterAction)), StartFilter)
                           .ObserveOnDispatcher()
                           .Bind(Favorites)
                           .Subscribe());
        }

        public IObservableCollection<ImageComponent> Files { get; } = new ObservableCollectionExtended<ImageComponent>();

        public IObservableCollection<ImageComponent> Filtered { get; } = new ObservableCollectionExtended<ImageComponent>();

        private readonly ReactiveProperty<string> _filter;
        public string Filter
        {
            get => _filter.Value;
            set => _filter.SetValueAndForceNotify(value);
        }

        public ObservableCommand StartFilter { get; } = new ObservableCommand();

        private bool FilterAction(ImageComponent component) => string.IsNullOrWhiteSpace(Filter) || component.Name.Contains(Filter);

        public IObservableCollection<ImageComponent> Favorites { get; } = new ObservableCollectionExtended<ImageComponent>();
    }
}