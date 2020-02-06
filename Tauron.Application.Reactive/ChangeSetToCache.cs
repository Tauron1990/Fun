using System;
using System.Collections.Generic;
using DynamicData;
using DynamicData.Annotations;
using DynamicData.Kernel;

namespace Tauron.Application.Reactive
{
    [PublicAPI]
    public sealed class ChangeSetToCache<TValue, TKey> : ISourceCache<TValue, TKey>
    {
        private readonly SourceCache<TValue, TKey> _cache;
        private readonly IDisposable _subscription;

        public ChangeSetToCache(Func<TValue, TKey> keySelector, IObservable<IChangeSet<TValue>> set)
        {
            _cache = new SourceCache<TValue, TKey>(keySelector);

            _subscription = set.Subscribe(cs =>
            {
                foreach (var change in cs)
                {
                    switch (change.Reason)
                    {
                        case ListChangeReason.Add:
                            _cache.AddOrUpdate(change.Item.Current);
                            break;
                        case ListChangeReason.AddRange:
                            _cache.AddOrUpdate(change.Range);
                            break;
                        case ListChangeReason.Replace:
                            change.Item.Previous.IfHasValue(tv => _cache.Remove(tv));
                            _cache.AddOrUpdate(change.Item.Current);
                            break;
                        case ListChangeReason.Remove:
                            _cache.Remove(change.Item.Current);
                            break;
                        case ListChangeReason.RemoveRange:
                            _cache.Remove(change.Range);
                            break;
                        case ListChangeReason.Refresh:
                            switch (change.Type)
                            {
                                case ChangeType.Item:
                                    _cache.Refresh(change.Item.Current);
                                    break;
                                case ChangeType.Range:
                                    _cache.Refresh(change.Range);
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            break;
                        case ListChangeReason.Clear:
                            _cache.Clear();
                            break;
                        case ListChangeReason.Moved:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            });
        }

        public IObservable<Change<TValue, TKey>> Watch(TKey key) 
            => _cache.Watch(key);

        public IObservable<IChangeSet<TValue, TKey>> Connect(Func<TValue, bool> predicate = null) 
            => _cache.Connect(predicate);

        public IObservable<IChangeSet<TValue, TKey>> Preview(Func<TValue, bool> predicate = null) 
            => _cache.Preview(predicate);

        public IObservable<int> CountChanged => _cache.CountChanged;

        public void Edit(Action<ISourceUpdater<TValue, TKey>> updateAction) 
            => _cache.Edit(updateAction);

        public void Dispose()
        {
            _cache.Dispose();
            _subscription.Dispose();
        }

        public Optional<TValue> Lookup(TKey key) 
            => _cache.Lookup(key);

        public IEnumerable<TKey> Keys => _cache.Keys;

        public IEnumerable<TValue> Items => _cache.Items;

        public IEnumerable<KeyValuePair<TKey, TValue>> KeyValues => _cache.KeyValues;

        public int Count => _cache.Count;
    }
}