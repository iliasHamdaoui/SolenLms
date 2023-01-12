using Imanys.SolenLms.Application.CourseManagement.Core.Domain.InstructorAggregate;

namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Instructors.Queries.GetAllInstructors;

internal static class InstructorExtension
{
    public static InstructorsListItem ToInstructorItem(this Instructor instructor)
    {
        return new InstructorsListItem(instructor.Id, instructor.FullName);
    }
}