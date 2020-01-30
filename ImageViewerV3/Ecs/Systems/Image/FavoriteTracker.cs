using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using EcsRx.Collections;
using EcsRx.Entities;
using EcsRx.Extensions;
using EcsRx.Groups;
using EcsRx.Groups.Observable;
using EcsRx.Plugins.Computeds.Collections;
using EcsRx.Systems;
using ImageViewerV3.Ecs.Blueprints.Data;
using ImageViewerV3.Ecs.Components.Data;
using ImageViewerV3.Ecs.Components.Image;

namespace ImageViewerV3.Ecs.Systems.Image
{
    public class FavoriteTrackerSystem : IManualSystem, IDisposable
    {
        private class FavoriteDataTracker : ComputedCollectionFromGroup<string>
        {
            private static readonly IGroup Group = new Group(typeof(DataFavoriteComponent), typeof(DataComponent));

            public FavoriteDataTracker(IEntityCollectionManager manager) 
                : base(manager.GetObservableGroup(Group, Collections.Data))
            {
            }

            public override IObservable<bool> RefreshWhen() 
                => InternalObservableGroup.OnEntityAdded.Merge(InternalObservableGroup.OnEntityRemoved).Select(_ => true);

            public override bool ShouldTransform(IEntity entity) 
                => true;

            public override string Transform(IEntity entity) => entity.GetComponent<DataComponent>().ReactiveValue.Value;
        }
        private class ImageTracker : ComputedCollectionFromGroup<(IEntity entity, ImageComponent imageComponent)>
        {
            private static readonly IGroup Group = new Group(typeof(ImageComponent));

            public ImageTracker(IEntityCollectionManager manager) 
                : base(manager.GetObservableGroup(Group, Collections.Images))
            {
            }

            public override IObservable<bool> RefreshWhen() 
                => InternalObservableGroup.OnEntityAdded.Merge(InternalObservableGroup.OnEntityRemoved).Select(_ => true);

            public override bool ShouldTransform(IEntity entity) 
                => true;

            public override (IEntity entity, ImageComponent imageComponent) Transform(IEntity entity) 
                => (entity, entity.GetComponent<ImageComponent>());
        }

        public IGroup Group { get; } = new Group(typeof(ImageComponent));

        private IDisposable? _subscription;
        
        private readonly FavoriteDataTracker _dataTracker;
        private readonly ImageTracker _imageTracker;

        public FavoriteTrackerSystem(IEntityCollectionManager manager)
        {
            _dataTracker = new FavoriteDataTracker(manager);
            _imageTracker = new ImageTracker(manager);
        }

        public void StartSystem(IObservableGroup observableGroup)
        {
            _subscription = new CompositeDisposable
            {
                _dataTracker
                    .OnAdded
                    .Select(cec => cec.NewValue)
                    .Select(s => _imageTracker.Value.FirstOrDefault(ec => ec.imageComponent.Name == s))
                    .Subscribe(p => p.entity.AddComponent(new IsFavoriteComponent())),

                _dataTracker
                    .OnRemoved
                    .Select(cec => cec.OldValue)
                    .Select(s => _imageTracker.Value.FirstOrDefault(ec => ec.imageComponent.Name == s))
                    .Where(e => e.entity.HasComponent<IsFavoriteComponent>())
                    .Subscribe(p => p.entity.RemoveComponent<IsFavoriteComponent>()),

                observableGroup
                    .OnEntityAdded
                    .Select(e => (e, e.GetComponent<ImageComponent>()))
                    .Where(p => _dataTracker.Contains(p.Item2.Name))
                    .Select(p => p.e)
                    .Subscribe(e => e.AddComponent(new IsFavoriteComponent()))
            };
        }

        public void StopSystem(IObservableGroup observableGroup) 
            => _subscription?.Dispose();

        public void Dispose()
        {
            _dataTracker.Dispose();
            _imageTracker.Dispose();
        }
    }
}