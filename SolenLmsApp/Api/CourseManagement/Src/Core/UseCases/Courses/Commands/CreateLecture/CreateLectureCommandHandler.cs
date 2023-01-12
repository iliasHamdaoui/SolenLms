﻿using Imanys.SolenLms.Application.CourseManagement.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core.Enums;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Events.CourseManagement.Courses;
using Imanys.SolenLms.Application.Shared.Core.UseCases;
using static Imanys.SolenLms.Application.Shared.Core.UseCases.RequestResponse<string>;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateLecture;

internal sealed class CreateLectureCommandHandler : IRequestHandler<CreateLectureCommand, RequestResponse<string>>
{
    private readonly IRepository<Course> _repository;
    private readonly IHashids _hashids;
    private readonly ILogger<CreateLectureCommandHandler> _logger;
    private readonly IIntegratedEventsSender _eventsSender;

    public CreateLectureCommandHandler(IRepository<Course> repository, IHashids hashids,
        ILogger<CreateLectureCommandHandler> logger, IIntegratedEventsSender eventsSender)
    {
        _repository = repository;
        _hashids = hashids;
        _logger = logger;
        _eventsSender = eventsSender;
    }

    public async Task<RequestResponse<string>> Handle(CreateLectureCommand command, CancellationToken _)
    {
        try
        {
            if (!TryDecodeCourseId(command.CourseId, out int courseId))
                return Error("Invalid course id.");

            if (!TryDecodeModuleId(command.ModuleId, out int moduleId))
                return Error("Invalid module id.");

            if (!TryParseLectureType(command.LectureType, out LectureType lectureType))
                return Error("The lecture type is invalid.");

            Course? course = await LoadCourseWithModuleFromRepository(courseId, moduleId);
            if (course is null)
                return Error("The course does not exist.");

            Module? module = GetModuleToAddLectureTo(moduleId, course);
            if (module is null)
                return Error("The module does not exist.");

            Lecture createdLecture = module.AddLecture(command.LectureTitle, lectureType);

            await SaveCourseToRepository(course, createdLecture);

            if (LectureTypeIsArticleOrVideo(createdLecture))
                await SendLectureWithResourceCreatedEvent(command, course, createdLecture);

            return Ok("The lecture has been created.", _hashids.Encode(createdLecture.Id));
        }
        catch (Exception ex)
        {
            return UnexpectedError("Error occured while creating the lecture.", ex);
        }
    }

    #region private methods

    private bool TryDecodeCourseId(string encodedCourseId, out int courseId)
    {
        if (_hashids.TryDecodeSingle(encodedCourseId, out courseId))
            return true;

        _logger.LogWarning("The encoded course id is invalid. encodedCourseId:{encodedCourseId}", encodedCourseId);
        return false;
    }

    private bool TryDecodeModuleId(string encodedModuleId, out int moduleId)
    {
        if (_hashids.TryDecodeSingle(encodedModuleId, out moduleId))
            return true;

        _logger.LogWarning("The encoded module id is invalid. encodedModuleId:{encodedModuleId}", encodedModuleId);
        return false;
    }

    private bool TryParseLectureType(string lectureTypeValue, out LectureType lectureType)
    {
        if (Enumeration.TryConvertFromValue(lectureTypeValue, out lectureType))
            return true;

        _logger.LogWarning("The lecture type value is invalid. lectureTypeValue:{lectureTypeValue}", lectureTypeValue);

        return false;
    }

    private Module? GetModuleToAddLectureTo(int moduleId, Course course)
    {
        Module? module = course.Modules.FirstOrDefault(x => x.Id == moduleId);
        if (module is null)
            _logger.LogWarning("The module does not exist. moduleId:{moduleId}", moduleId);

        return module;
    }

    private async Task<Course?> LoadCourseWithModuleFromRepository(int courseId, int moduleId)
    {
        Course? course = await _repository.FirstOrDefaultAsync(new GetCourseWithModuleSpec(courseId, moduleId));
        if (course is null)
            _logger.LogWarning("The course does not exist. courseId:{courseId}", courseId);

        return course;
    }

    private async Task SaveCourseToRepository(Course? course, Lecture createdLecture)
    {
        await _repository.UpdateAsync(course!);

        _logger.LogInformation("Lecture created. lectureId:{lectureId}, encodedLectureId:{encodedLectureId}",
            createdLecture.Id, _hashids.Encode(createdLecture.Id));
    }

    private async Task SendLectureWithResourceCreatedEvent(CreateLectureCommand command, Course course,
        Lecture createdLecture)
    {
        LectureWithResourceCreated @event = new()
        {
            OrganizationId = course.OrganizationId,
            CourseId = command.CourseId,
            ModuleId = command.ModuleId,
            LectureId = _hashids.Encode(createdLecture.Id),
            MediaType = createdLecture.Type.MediaType
        };

        await _eventsSender.SendEvent(@event);
    }

    private static bool LectureTypeIsArticleOrVideo(Lecture createdLecture)
    {
        return createdLecture.Type.Value == LectureType.Article.Value ||
               createdLecture.Type.Value == LectureType.Video.Value;
    }

    private RequestResponse<string> UnexpectedError(string error, Exception exception)
    {
        _logger.LogError(exception, "Error occured while creating the lecture. message:{message}", exception.Message);
        return Error(ResponseError.Unexpected, error);
    }

    private sealed class GetCourseWithModuleSpec : SingleResultSpecification<Course>
    {
        public GetCourseWithModuleSpec(int courseId, int moduleId)
        {
            Query
                .Where(x => x.Id == courseId)
                .Include(x => x.Modules.Where(module => module.Id == moduleId))
                .ThenInclude(x => x.Lectures);
        }
    }

    #endregion
}