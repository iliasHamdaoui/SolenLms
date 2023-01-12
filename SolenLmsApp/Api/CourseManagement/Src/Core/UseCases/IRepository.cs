namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases;

internal interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}