using System;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using DynamicData;
using DynamicData.Alias;
using DynamicData.Binding;
using DynamicData.Kernel;
using ImageViewerV3.Core;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ui.Services
{
    public sealed class FilesManager : EcsConnector
    {
        public FilesManager(IListManager listManager, IEventSystem eventSystem, IImageIndexer imageIndexer)
            : base(listManager, eventSystem)
        {
            ReactOn<PrepareLoadEvent>(_ => Filter = string.Empty);
            ReactOn<DeleteEvent>(c =>
                                 {
                                     try
                                     {
                                         imageIndexer
                                            .GetEntity(c.Index)
                                            .IfHasValue(ic =>
                                                        {
                                                            File.Delete(ic.FilePath);
                                                            imageIndexer.Remove(ic.Index);
                                                        });
                                     }
                                     catch (Exception e)
                                     {
                                         MessageBox.Show(e.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                                     }
                                 });

            var imageSource = listManager.GetList<ImageComponent>();

            _filter = Track(new ReactiveProperty<SearchQuery>(), nameof(Filter));

            var filter = _filter.Select(sq => new Func<ImageComponent, bool>(sq.FilterAction));

            DisposeThis(imageSource
                           .Connect()
                           .Filter(filter)
                           .ObserveOnDispatcher()
                           .Bind(Filtered)
                           .Subscribe());

            DisposeThis(imageSource
                           .Connect()
                           .AutoRefreshOnObservable(ic => ic.IsFavorite)
                           .Where(c => c.IsFavorite.Value)
                           .ObserveOnDispatcher()
                           .Bind(Favorites)
                           .Subscribe());
        }

        //public IObservableCollection<ImageComponent> Files { get; } = new ObservableCollectionExtended<ImageComponent>();

        public IObservableCollection<ImageComponent> Filtered { get; } = new ObservableCollectionExtended<ImageComponent>();

        private readonly ReactiveProperty<SearchQuery> _filter;

        public IObservable<string> FilterObservable => _filter.Select(sc => sc.Term);

        public string Filter
        {
            get => _filter.Value?.Term ?? string.Empty;
            set => _filter.Value = SearchQuery.ParseTerm(value);
        }

        public IObservableCollection<ImageComponent> Favorites { get; } = new ObservableCollectionExtended<ImageComponent>();
    }
}