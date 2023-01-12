namespace Imanys.SolenLms.Application.Resources.Core.UseCases;

internal interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}
