using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.LearnersProgress.Queries.GetLearnersProgress;

public sealed record GetLearnersProgressQuery(string CourseId) : IRequest<RequestResponse<GetLearnersProgressQueryResult>>;
