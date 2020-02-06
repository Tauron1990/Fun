using System;
using DynamicData;
using JetBrains.Annotations;

namespace Tauron.Application.Reactive
{
    [PublicAPI]
    public interface IListManager
    {
        ISourceList<TType> GetList<TType>(int key = 0)
                where TType : IEntity;


        IObservable<IChangeSet<IEntity>> GetUnversalList();
    }
}