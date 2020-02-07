using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DynamicData;
using JetBrains.Annotations;

namespace Tauron.Application.Reactive
{
    public abstract class ReactToEntitySystem<TType> : ISystem, IDisposable
        where TType : IEntity
    {
        private class ReferenceComparer : IEqualityComparer<TType>
        {
            public bool Equals(TType x, TType y)
                => ReferenceEquals(x, y);

            public int GetHashCode(TType obj)
                => RuntimeHelpers.GetHashCode(obj);
        }

        private readonly object _gate = new object();
        private readonly ConcurrentDictionary<TType, IDisposable> _subscriptions = new ConcurrentDictionary<TType, IDisposable>(new ReferenceComparer());

        private IDisposable? _lisSubscription;
        
        protected abstract IObservable<TType> ReactTo(TType entity);

        protected abstract void Process(TType entity);

        public void Init(IListManager listManager)
        {
            var list = listManager.GetList<TType>();

            _lisSubscription = list.Connect().OnItemAdded(OnAdd).OnItemRemoved(OnRemove).Subscribe();
        }

        private void OnRemove(TType obj)
        {
            if (_subscriptions.Remove(obj, out var sub))
                sub.Dispose();
        }

        private void OnAdd(TType obj)
        {
            var subscription = ReactTo(obj).Subscribe(Process);
            _subscriptions[obj] = subscription;
        }

        public void Dispose()
        {
            lock (_gate)
            {
                _lisSubscription?.Dispose();
                _lisSubscription = null;

                foreach (var disposable in _subscriptions.Values)
                    disposable.Dispose();

                _subscriptions.Clear();
            }
        }
    }
}