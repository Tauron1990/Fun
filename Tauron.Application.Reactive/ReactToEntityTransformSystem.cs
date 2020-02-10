using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DynamicData;

namespace Tauron.Application.Reactive
{
    public abstract class ReactToEntityTransformSystem<TType, TResult> : ISystem, IDisposable
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

        protected abstract IObservable<TResult> ReactTo(TType entity);

        protected abstract void Process(TResult entity);

        public void Init(IListManager listManager)
        {
            var list = listManager.GetList<TType>();

            _lisSubscription = list.Connect().OnItemAdded(OnAdd).OnItemRemoved(OnRemove).Subscribe();
        }

        protected virtual void OnRemove(TType obj)
        {
            if (_subscriptions.Remove(obj, out var sub))
                sub.Dispose();
        }

        protected virtual void OnAdd(TType obj)
        {
            var subscription = ReactTo(obj).Subscribe(Process);
            _subscriptions[obj] = subscription;
        }

        public virtual void Dispose()
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