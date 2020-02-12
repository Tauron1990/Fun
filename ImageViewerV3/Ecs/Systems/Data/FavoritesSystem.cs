using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using DynamicData.Kernel;
using ImageViewerV3.Data;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Systems.Data
{
    public sealed class FavoritesSystem : ReactToEntityTransformSystem<ImageComponent, (bool favorite, ImageComponent imageComponent)>
    {
        private readonly IFolderConfiguration _folderConfiguration;
        private readonly IDisposable _eventSubscription;
        private readonly IReadOnlyCollection<string> _favorites;

        public FavoritesSystem(IEventSystem eventSystem, IImageIndexer indexer, IFolderConfiguration folderConfiguration, IListManager listManager)
        {
            _folderConfiguration = folderConfiguration;
            _favorites = folderConfiguration.Favorites;

            var list = listManager.GetList<ImageComponent>();

            _eventSubscription = eventSystem
               .Receive<ToogleFavoritesEvent>()
               .Subscribe(e =>
                          {
                              var component = indexer.GetEntity(e.Index);
                              component.IfHasValue(ic => ic.IsFavorite.Value = !ic.IsFavorite.Value);
                          });
        }

        protected override IObservable<(bool favorite, ImageComponent imageComponent)> ReactTo(ImageComponent entity)
        {
            if (_favorites.Any(f => entity.Name == f))
                entity.IsFavorite.Value = true;

            return entity.IsFavorite.Select(e => (e, entity));
        }

        protected override void Process((bool favorite, ImageComponent imageComponent) entity)
        {
            var (favorite, imageComponent) = entity;
            imageComponent.IsFavorite.Value = favorite;
            _folderConfiguration.ToggleFavorite(imageComponent.Name, favorite);
        }

        public override void Dispose()
        {
            _eventSubscription.Dispose();
            base.Dispose();
        }
    }
}