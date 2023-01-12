namespace Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Instructors.Queries.GetAllInstructors;

public sealed record GetAllInstructorsQueryResult(List<InstructorsListItem> Referents);

public sealed record InstructorsListItem(string Id, string Name);
