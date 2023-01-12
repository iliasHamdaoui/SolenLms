namespace Imanys.SolenLms.Application.Resources.Core.UseCases;
internal interface IMediaManager
{
    Task<MediaUploadResult> Upload(IResourceFile resourceFile, string organizationId, string courseId, string moduleId, string lectureId);
    Task DeleteLectureMedias(string organizationId, string courseId, string moduleId, string lectureId);
    Task DeleteModuleMedias(string organizationId, string courseId, string moduleId);
    Task DeleteCourseMedias(string organizationId, string courseId);
    Task DeleteOrganizationMedias(string organizationId);
    Task<byte[]> GetMediaContent(string? mediaPath);
    Task<Stream?> GetMediaContentStream(string? mediaPath);
}