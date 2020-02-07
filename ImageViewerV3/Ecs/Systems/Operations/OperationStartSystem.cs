using DynamicData;
using ImageViewerV3.Ecs.Components;
using ImageViewerV3.Ecs.Events;
using Tauron.Application.Reactive;

namespace ImageViewerV3.Ecs.Systems.Operations
{
    public sealed class OperationStartSystem : EventReactionSystem<StartOperationEvent>
    {
        private readonly ISourceList<OperationComponent> _operations;

        public OperationStartSystem(IEventSystem eventSystem, IListManager listManager) 
            : base(eventSystem) =>
            _operations = listManager.GetList<OperationComponent>();

        protected override void EventTriggered(StartOperationEvent eventData) 
            => _operations.Add(new OperationComponent(eventData.Name, eventData.ToDo, eventData.Data));
    }
}