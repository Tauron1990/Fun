using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
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
using ImageViewerV3.Ecs.Components.Data;
using ImageViewerV3.Ecs.Components.Image;
using ImageViewerV3.Ecs.Events;

namespace ImageViewerV3.Ecs.Services
{
    public sealed class FolderConfiguration : IFolderConfiguration, IDisposable
    {
        private const string PagingType = "Paging";

        private class IndexBlueprint : IBlueprint
        {
            public const string IndexName = "Index";

            public void Apply(IEntity entity)
            {
                entity.AddComponent(new TypeComponent(PagingType));
                entity.AddComponent(new DataComponent(IndexName, "0"));
            }
        }

        private class SelecedImageProperty : ComputedFromGroup<Optional<SelecedImage>>
        {
            private static readonly IGroup Group = new Group(typeof(SelecedImageComponent));

            public SelecedImageProperty(IEntityCollectionManager manager)
                : base(manager.GetObservableGroup(Group, Collections.Images))
            {
            }

            public override IObservable<bool> RefreshWhen()
                => InternalObservableGroup.OnEntityAdded.Merge(InternalObservableGroup.OnEntityRemoved).Select(_ => true);

            public override Optional<SelecedImage> Transform(IObservableGroup observableGroup)
            {
                return observableGroup
                   .Where(e => e.HasComponent<SelecedImageComponent>())
                   .Select(e =>
                           {
                               var comp = e.GetComponent<ImageComponent>();
                               return new SelecedImage(comp.Name, comp.FilePath, e.HasComponent<IsFavoriteComponent>());
                           })
                   .FirstOrOptional(si => true);
            }
        }
        private class DataProperty : ComputedFromGroup<Optional<DataComponent>>
        {
            private static readonly IGroup Group = new Group(typeof(TypeComponent), typeof(DataComponent));

            private readonly string _type;
            private readonly string _name;

            public DataProperty(IEntityCollectionManager manager, string type, string name) 
                : base(manager.GetObservableGroup(Group, Collections.Data))
            {
                _type = type;
                _name = name;
            }

            public override IObservable<bool> RefreshWhen() 
                => InternalObservableGroup.OnEntityRemoved.Merge(InternalObservableGroup.OnEntityAdded).Select(_ => true);

            public override Optional<DataComponent> Transform(IObservableGroup observableGroup)
            {
                return observableGroup
                   .Select(e => (e.GetComponent<TypeComponent>().Name, e.GetComponent<DataComponent>()))
                   .Where(c => c.Name == _type && c.Item2.Name == _name)
                   .Select(c => c.Item2)
                   .FirstOrOptional(_ => true);
            }
        }

        private static readonly Dictionary<string, IBlueprint> Blueprints = new Dictionary<string, IBlueprint>
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