using Fluxor;
using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Learning.LearningCourse.Store;

public sealed record LearningState
{
    public GetCourseToLearnByIdQueryResult? Course { get; init; }
    public LectureForGetCourseToLearnByIdQueryResult? CurrentLecture { get; set; }
}

public sealed class LearningStateFeatureState : Feature<LearningState>
{
    public override string GetName() => nameof(LearningState);

    protected override LearningState GetInitialState()
    {
        return new LearningState();
    }
}
