using System;

namespace Tauron.Application.Reactive
{
    public abstract class EventReactionSystem<TEvent> : ISystem, IDisposable
    {
        protected  IEventSystem EventSystem { get; }

        private IDisposable? _subscription;

        protected EventReactionSystem(IEventSystem eventSystem) 
            => EventSystem = eventSystem;

        public void Init(IListManager listManager) 
            => _subscription = EventSystem.Receive<TEvent>().Subscribe(EventTriggered);

        protected abstract void EventTriggered(TEvent data);
        public void Dispose() 
            => _subscription.Dispose();
    }
}