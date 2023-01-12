namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UploadLectureVideo;
internal interface IStorageRepo
{
    Task<long> GetCurrentStorageRepo(string organizationId, CancellationToken cancellationToken);
}
