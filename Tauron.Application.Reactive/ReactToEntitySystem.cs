namespace Tauron.Application.Reactive
{
    public abstract class ReactToEntitySystem<TType> : ReactToEntityTransformSystem<TType, TType>
        where TType : IEntity
    {
    }
}