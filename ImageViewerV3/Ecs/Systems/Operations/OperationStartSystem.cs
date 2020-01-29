using EcsRx.Collections;
using EcsRx.Events;
using EcsRx.Plugins.ReactiveSystems.Custom;
using ImageViewerV3.Ecs.Blueprints;
using ImageViewerV3.Ecs.Events;

namespace ImageViewerV3.Ecs.Systems.Operations
{
    public sealed class OperationStartSystem : EventReactionSystem<StartOperationEvent>
    {
        private readonly IEntityCollection _entitys;

        public OperationStartSystem(IEventSystem eventSystem, IEntityCollectionManager collectionManager) 
            : base(eventSystem) =>
            _entitys = collectionManager.GetCollection(Collections.Gui);

        public override void EventTriggered(StartOperationEvent eventData) 
            => _entitys.CreateEntity(new OperationBlueprint(eventData));
    }
}