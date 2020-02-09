using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Tauron.Application.Reactive
{
    public class EventSystem : IEventSystem
    {
        private class ObserfEntry
        {
            private readonly object _target;

            private ObserfEntry(object target)
                => _target = target;

            public static ObserfEntry Create<TType>()
                => new ObserfEntry(new Subject<TType>());

            public Subject<TType> Get<TType>()
                => (Subject<TType>) _target;
        }

        private ConcurrentDictionary<Type, ObserfEntry> _cache = new ConcurrentDictionary<Type, ObserfEntry>();

        public void Publish<TEvent>(TEvent @event)
        {
            if (_cache.TryGetValue(typeof(TEvent), out var target)) 
                target.Get<TEvent>().OnNext(@event);
        }

        public IObservable<TEvent> Receive<TEvent>() 
            => _cache.GetOrAdd(typeof(TEvent), _ => ObserfEntry.Create<TEvent>()).Get<TEvent>().AsObservable();
    }
}