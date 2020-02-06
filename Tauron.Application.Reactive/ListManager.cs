using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using DynamicData;

namespace Tauron.Application.Reactive
{
    public sealed class ListManager : IListManager, IDisposable
    {
        private sealed class ListKey : IEquatable<ListKey>
        {
            public int Key { get; }

            public Type TargetType { get; }

            public ListKey(int key, Type targetType)
            {
                Key = key;
                TargetType = targetType;
            }

            public override bool Equals(object obj) => base.Equals(obj);

            public bool Equals(ListKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Key == other.Key && TargetType.Equals(other.TargetType);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (Key * 397) ^ TargetType.GetHashCode();
                }
            }

            public static bool operator ==(ListKey left, ListKey right) => Equals(left, right);

            public static bool operator !=(ListKey left, ListKey right) => !Equals(left, right);
        }

        private bool _isDisposed;

        private readonly object _lock = new object();
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private readonly Subject<IChangeSet<IEntity>> _universalSubject = new Subject<IChangeSet<IEntity>>();

        private readonly ConcurrentDictionary<ListKey, object> _lists = new ConcurrentDictionary<ListKey, object>();

        public ListManager() 
            => _compositeDisposable.Add(_universalSubject);

        public ISourceList<TType> GetList<TType>(int key = 0) 
            where TType : IEntity
        {
            CheckDisposed();

            var listKey = new ListKey(key, typeof(TType));

            while (true)
            {
                if (_lists.TryGetValue(listKey, out var target))
                    return (ISourceList<TType>) target;

                var list = new SourceList<TType>().Connect().Bind;

                if (_lists.TryAdd(listKey, list))
                {
                    UpdateSourceLists(listKey, list);
                    return list;
                }

                list.Dispose();
            }
        }

        public IObservable<IChangeSet<IEntity>> GetUnversalList() 
            => _universalSubject;


        private void UpdateSourceLists<TType>(ListKey key, SourceList<TType> list) 
            => _compositeDisposable.Add(list.Connect().Cast(t => (IEntity)t).Subscribe(_universalSubject));

        private void CheckDisposed()
        {
            if(_isDisposed)
                throw new ObjectDisposedException(nameof(ListManager));
        }

        public void Dispose()
        {
            lock (_lock)
            {
                if(_isDisposed) return;
                _isDisposed = true;
            }

            _compositeDisposable.Dispose();
            _lists.Clear();
        }
    }
}