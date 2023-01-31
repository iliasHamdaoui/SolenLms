namespace Imanys.SolenLms.Application.CourseManagement.Features;

internal interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregateRoot
{
}