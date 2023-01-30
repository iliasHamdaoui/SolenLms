namespace Imanys.SolenLms.Application.Resources.Features;

internal interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}
