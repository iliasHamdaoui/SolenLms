using System.Text.Json.Serialization;
using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UpdateLectureArticle;

public sealed record UpdateLectureArticleCommand : IRequest<RequestResponse>
{
    [JsonIgnore]
    public string ResourceId { get; set; } = default!;
    public string? Content { get; set; }
}
