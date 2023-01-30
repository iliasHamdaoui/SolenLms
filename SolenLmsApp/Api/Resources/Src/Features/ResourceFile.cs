using System.Net.Http.Headers;

namespace Imanys.SolenLms.Application.Resources.Features;

public sealed class ResourceFile : IResourceFile
{
    private readonly IFormFile _formFile;

    public ResourceFile(IFormFile formFile)
    {
        _formFile = formFile ?? throw new ArgumentNullException(nameof(formFile));

        ContentType = formFile.ContentType;
        FileName = formFile.FileName;
        Length = formFile.Length;
        Name = formFile.Name;

        FileExtension =
            Path.GetExtension(ContentDispositionHeaderValue.Parse(formFile.ContentDisposition).FileName!.Trim('"'));
    }

    public string ContentType { get; }
    public string FileExtension { get; }
    public string FileName { get; }
    public long Length { get; }
    public string Name { get; }

    public Stream OpenReadStream()
    {
        return _formFile.OpenReadStream();
    }

    public void CopyTo(Stream target)
    {
        _formFile.CopyTo(target);
    }

    public Task CopyToAsync(Stream target)
    {
        return _formFile.CopyToAsync(target);
    }
}