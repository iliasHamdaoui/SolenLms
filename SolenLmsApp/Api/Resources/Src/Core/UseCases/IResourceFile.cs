namespace Imanys.SolenLms.Application.Resources.Core.UseCases;

public interface IResourceFile
{
    string ContentType { get; }
    string FileExtension { get; }
    string FileName { get; }
    long Length { get; }
    string Name { get; }
    void CopyTo(Stream target);
    Task CopyToAsync(Stream target);
    Stream OpenReadStream();
}
