using System;

namespace Tauron.Application.Reactive
{
    public class EventSystem : IEventSystem
    {
        public void Publish<TEvent>(TEvent @event)
        {
            throw new NotImplementedException();
        }

        public IObservable<TEvent> Subscribe<TEvent>() => throw new NotImplementedException();
    }
}