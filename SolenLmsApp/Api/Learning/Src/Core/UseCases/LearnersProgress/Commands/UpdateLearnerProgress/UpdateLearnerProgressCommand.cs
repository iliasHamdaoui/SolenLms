using Imanys.SolenLms.Application.Shared.Core.UseCases;

namespace Imanys.SolenLms.Application.Learning.Core.UseCases.LearnersProgress.Commands.UpdateLearnerProgress;

public sealed record UpdateLearnerProgressCommand
    (string CourseId, string LectureId, bool LastLecture) : IRequest<RequestResponse>;