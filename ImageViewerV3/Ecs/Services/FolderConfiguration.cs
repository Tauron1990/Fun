using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Controls;
using DynamicData.Kernel;
using EcsRx.Blueprints;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Events;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.Computeds;
using EcsRx.ReactiveData;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Reactive.Bindings;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Services
{
    public sealed class FolderConfiguration : IFolderConfiguration, IDisposable
    {
        private const string PagingType = "Paging";

        private class IndexBlueprint
        {
            public const string IndexName = "Index";

            public DataComponent Apply()
            {
                return new DataComponent(IndexName, "0", PagingType);

                entity.AddComponent(new TypeComponent(PagingType));
                entity.AddComponent(new DataComponent(IndexName, "0"));
            }
        }

        private static readonly Dictionary<string, Func<DataComponent>> Blueprints = new Dictionary<string, IBlueprint>
                                                                             {
                                                                                 { PagingType+IndexBlueprint.IndexName, new IndexBlueprint() }
                                                                             };

        private readonly CompositeDisposable _disposable = new CompositeDisposable();
        private readonly IEntityCollection _dataCollection;

        public IComputed<Optional<SelecedImage>> SelectedImage { get; }

        public ReactiveProperty<int> CurrentIndex { get; } = new ReactiveProperty<int>(0);


        public FolderConfiguration(IEntityCollectionManager entityCollectionManager, IEventSystem eventSystem)
        {
            _disposable.Add(eventSystem.Receive<PostLoadingEvent>().Subscribe(CheckSettings));

            SelectedImage = new SelecedImageProperty(entityCollectionManager);
            _dataCollection = entityCollectionManager.GetCollection(Collections.Data);

            var indexProp = new DataProperty(entityCollectionManager, PagingType, IndexBlueprint.IndexName);
            _disposable.Add(
                CurrentIndex.Subscribe(
                    index => indexProp.Value.IfHasValue(c => c.ReactiveValue?.SetValueAndForceNotify(index.ToString()))));

            _disposable.Add(CurrentIndex);
        }



        private void CheckSettings(PostLoadingEvent e)
        {
            var checkedList = new HashSet<string>();

            foreach (var entity in _dataCollection)
            {
                var types = entity.GetComponent<TypeComponent>();
                var comp = entity.GetComponent<DataComponent>();

                switch (types.Name)
                {
                    case PagingType:
                        switch (comp.Name)
                        {
                            case IndexBlueprint.IndexName:
                                if (int.TryParse(comp.ReactiveValue.Value, out var index))
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
                _dataCollection.CreateEntity(blueprint.Value);
        }

        public void Dispose() 
            => _disposable.Dispose();
    }
}