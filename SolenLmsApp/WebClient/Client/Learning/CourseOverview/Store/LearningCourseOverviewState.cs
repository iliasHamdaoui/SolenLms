using Fluxor;
using Imanys.SolenLms.Application.WebClient.Learning.Shared.Services;

namespace Imanys.SolenLms.Application.WebClient.Learning.CourseOverview.Store;

public sealed record LearningCourseOverviewState
{
    public GetCourseByIdQueryResult? CurrentCourse { get; init; }

}


public sealed class LearningCourseOverviewFeatureState : Feature<LearningCourseOverviewState>
{
    public override string GetName() => nameof(LearningCourseOverviewState);

    protected override LearningCourseOverviewState GetInitialState()
    {
        return new LearningCourseOverviewState();
    }
}


