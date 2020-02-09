using System;
using System.Diagnostics;
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
        public IImageIndexer ImageIndexer { get; }


        public FilesManager(IListManager listManager, IEventSystem eventSystem, IImageIndexer imageIndexer)
            : base(listManager, eventSystem)
        {
            ImageIndexer = imageIndexer;
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

            _filter = Track(new ReactiveProperty<string>(), nameof(Filter));

            var filter = Observable.Return(new Func<ImageComponent, bool>(FilterAction)).RepeatWhen(_ => _filter);

            DisposeThis(imageSource
                           .Connect()
                           .Filter(filter)
                           .ObserveOnDispatcher()
                           .Bind(Filtered)
                           .Subscribe());

            DisposeThis(imageSource
                           .Connect()
                           .Where(c => c.IsFavorite.Value)
                           .Filter(filter)
                           .ObserveOnDispatcher()
                           .Bind(Favorites)
                           .Subscribe());
        }

        //public IObservableCollection<ImageComponent> Files { get; } = new ObservableCollectionExtended<ImageComponent>();

        public IObservableCollection<ImageComponent> Filtered { get; } = new ObservableCollectionExtended<ImageComponent>();

        private readonly ReactiveProperty<string> _filter;

        public IObservable<string> FilterObservable => _filter.AsObservable();

        public string Filter
        {
            get => _filter.Value;
            set => _filter.Value = value;
        }

        [DebuggerStepThrough]
        private bool FilterAction(ImageComponent component) => string.IsNullOrWhiteSpace(Filter) || component.Name.Contains(Filter);

        public IObservableCollection<ImageComponent> Favorites { get; } = new ObservableCollectionExtended<ImageComponent>();
    }
}