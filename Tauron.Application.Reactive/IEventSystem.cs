using System;
using DynamicData.Annotations;

namespace Tauron.Application.Reactive
{
    [PublicAPI]
    public interface IEventSystem
    {
        void Publish<TEvent>(TEvent @event);

        IObservable<TEvent> Subscribe<TEvent>();
    }
}