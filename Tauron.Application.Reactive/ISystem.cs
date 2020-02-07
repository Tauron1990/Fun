using System;

namespace Tauron.Application.Reactive
{
    public interface ISystem
    {
        void Init(IListManager listManager);
    }
}