using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;

namespace Tauron.Application.Reactive
{
    public sealed class ListManager : IListManager, IDisposable
    {
        private sealed class ListKey : IEquatable<ListKey>
        {
            private int Key { get; }

            private Type TargetType { get; }

            public ListKey(int key, Type targetType)
            {
                Key = key;
                TargetType = targetType;
            }

            public bool Equals(ListKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Key == other.Key && TargetType == other.TargetType;
            }

            public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is ListKey other && Equals(other);

            public override int GetHashCode() => HashCode.Combine(Key, TargetType);

            public static bool operator ==(ListKey left, ListKey right) => Equals(left, right);

            public static bool operator !=(ListKey left, ListKey right) => !Equals(left, right);
        }

        private bool _isDisposed;

        private readonly object _lock = new object();
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private readonly ConcurrentDictionary<ListKey, object> _lists = new ConcurrentDictionary<ListKey, object>();
        
        public ISourceList<TType> GetList<TType>(int key = 0) 
            where TType : IEntity
        {
            CheckDisposed();

            var listKey = new ListKey(key, typeof(TType));

            while (true)
            {
                if (_lists.TryGetValue(listKey, out var target))
                    return (ISourceList<TType>) target;

                var list = new SourceList<TType>();
                
                if (_lists.TryAdd(listKey, list))
                    return list;

                list.Dispose();
            }
        }
        
        
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