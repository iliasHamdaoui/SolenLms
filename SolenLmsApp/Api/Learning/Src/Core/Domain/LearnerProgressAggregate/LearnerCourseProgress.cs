﻿using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Learning.Core.Domain.LearnerAggregate;
using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.LearnerProgressAggregate;

public sealed class LearnerCourseProgress : IAggregateRoot
{
    public string LearnerId { get; init; }
    public Learner Learner { get; init; } = default!;
    public string CourseId { get; init; }
    public Course Course { get; init; } = default!;
    public float Progress { get; private set; }
    public DateTime LastAccessTime { get; private set; }


    public LearnerCourseProgress(string learnerId, string courseId)
    {
        ArgumentNullException.ThrowIfNull(learnerId, nameof(learnerId));
        ArgumentNullException.ThrowIfNull(courseId, nameof(courseId));

        LearnerId = learnerId;
        CourseId = courseId;
    }

    public void UpdateProgress(float progress, DateTime lastAccessTime)
    {
        Progress = progress;
        LastAccessTime = lastAccessTime;
    }
}