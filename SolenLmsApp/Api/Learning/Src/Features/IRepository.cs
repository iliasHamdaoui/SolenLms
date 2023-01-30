namespace Imanys.SolenLms.Application.Learning.Features;

internal interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}
