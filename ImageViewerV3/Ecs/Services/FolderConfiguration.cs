using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Services
{
    public sealed class FolderConfiguration : IFolderConfiguration, IDisposable
    {
        private const string PagingType = "Paging";

        private const string FavoriteType = "Favorite";

        private class DualLink<TType> : IDisposable
        {
            private readonly IDisposable _toproperty;
            private readonly IDisposable _toData;

            public DualLink(Func<string, (bool IsOk, TType Result)> converter, IReactiveProperty<TType> reactiveProperty, DataComponent data)
            {
                _toData = reactiveProperty.Subscribe(n => data.ReactiveValue.Value = n?.ToString()!);
                _toproperty = data.ReactiveValue.Select(converter).Subscribe(res =>
                {
                    var (isOk, result) = res;
                    if (isOk)
                        reactiveProperty.Value = result;
                });
            }

            public void Dispose()
            {
                _toproperty.Dispose();
                _toData.Dispose();
            }
        }

        private interface IBlueprint
        {
            DataComponent Create();
        }

        private class IndexBlueprint : IBlueprint
        {
            public const string IndexName = "Index";

            public DataComponent Create() 
                => new DataComponent(IndexName, "0", PagingType);
        }

        private static readonly Random _idGen = new Random();

        private static readonly Dictionary<string, IBlueprint> Blueprints = new Dictionary<string, IBlueprint>
                                                                             {
                                                                                 { PagingType+IndexBlueprint.IndexName, new IndexBlueprint() }
                                                                             };

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly ISourceList<DataComponent> _dataCollection;
        private readonly Func<DataComponent> _indexData;

        private CompositeDisposable? _loadDispose;

        public ReactiveProperty<int> CurrentIndex { get; } = new ReactiveProperty<int>(0);
        public IReadOnlyCollection<string> Favorites { get; }

        public void ToggleFavorite(string name, bool value)
        {
            var dc = _dataCollection.Items.FirstOrDefault(dc => dc.Name == name);

            if (dc == null && value)
                _dataCollection.Add(new DataComponent(name, value.ToString(), FavoriteType));
            else
                dc.ReactiveValue.Value = value.ToString();
        }


        public FolderConfiguration(IListManager listManager, IEventSystem eventSystem, IImageIndexer indexer)
        {
            _disposable.Add(eventSystem.Receive<PostLoadingEvent>().Subscribe(CheckSettings));

            _dataCollection = listManager.GetList<DataComponent>();
            _indexData = () => _dataCollection.Items.First(dc => dc.Category == PagingType && dc.Name == IndexBlueprint.IndexName);

            _disposable.Add(CurrentIndex);
            
            _disposable.Add(
                eventSystem
                   .Receive<DeleteEvent>()
                   .Select(e => indexer.GetEntity(e.Index))
                   .Where(o => o.HasValue)
                   .Select(o => o.Value)
                   .Select(ic => _dataCollection.Items.FirstOrDefault(dc => dc.Name == ic.Name))
                   .Where(dc => dc != null)
                   .Subscribe(dc => _dataCollection.Remove(dc)));

            _disposable.Add(
                _dataCollection
                   .Connect(dc => dc.Category == FavoriteType)
                   .Filter(dc => bool.TryParse(dc.ReactiveValue.Value, out var result) && result)
                   .Transform(dc => dc.Name)
                   .Bind(out var list).Subscribe());

            Favorites = list;
        }



        private void CheckSettings(PostLoadingEvent e)
        {
            _loadDispose?.Dispose();


            var checkedList = new HashSet<string>();

            foreach (var item in _dataCollection.Items)
            {
                switch (item.Category)
                {
                    case PagingType:
                        switch (item.Name)
                        {
                            case IndexBlueprint.IndexName:
                                if (int.TryParse(item.ReactiveValue.Value, out var index))
                                {
                                    CurrentIndex.Value = index;
                                    checkedList.Add(PagingType + IndexBlueprint.IndexName);
                                }
                                break;
                        }
                        break;
                }
            }

            foreach (var blueprint in Blueprints.Where(blueprint => checkedList.Add(blueprint.Key))) 
                _dataCollection.Add(blueprint.Value.Create());

            _loadDispose = new CompositeDisposable
            {
                Link(s =>
                {
                    var isOk = int.TryParse(s, out var result);
                    return (isOk, result);
                }, CurrentIndex, _indexData())
            };
        }

        private static IDisposable Link<TType>(Func<string, (bool IsOk, TType Result)> converter, IReactiveProperty<TType> reactiveProperty, DataComponent data)
            => new DualLink<TType>(converter, reactiveProperty, data);

        public void Dispose()
        {
            _loadDispose?.Dispose();
            _disposable.Dispose();
        }
    }
}