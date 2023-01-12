namespace Imanys.SolenLms.Application.Shared.Core;

public sealed record ResponseError
{
    public int Code { get; }
    public string Title { get; }
    public string Type { get; }

    public static readonly ResponseError NotFound = new(404, "Not found", "https://tools.ietf.org/html/rfc7231#section-6.5.4");
    public static readonly ResponseError BadRequest = new(400, "Bad Request", "https://tools.ietf.org/html/rfc7231#section-6.5.1");
    public static readonly ResponseError Unprocessable = new(422, "Unprocessable Entity", "https://tools.ietf.org/html/rfc4918#section-11.2");
    public static readonly ResponseError Unexpected = new(500, "Unexpected Error", "https://tools.ietf.org/html/rfc7231#section-6.6.1");


    private ResponseError(int code, string title, string type)
    {
        Code = code;
        Title = title;
        Type = type;
    }
}
