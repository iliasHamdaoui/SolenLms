namespace Imanys.SolenLms.Application.Learning.Core.UseCases;

internal interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}
