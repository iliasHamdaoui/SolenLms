namespace Imanys.SolenLms.Application.Learning.Core.UseCases.Courses.Queries.GetFilters;

public sealed record GetFiltersQueryResult
{
    public IEnumerable<InstructorForGetFiltersQueryResult> Instructors { get; set; } = default!;
    public IEnumerable<CategoryForGetFiltersQueryResult> Categories { get; set; } = default!;
}


public sealed record InstructorForGetFiltersQueryResult
{
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;

    public InstructorForGetFiltersQueryResult(string id, string name)
    {
        Id = id;
        Name = name;
    }
}

public sealed record CategoryForGetFiltersQueryResult
{
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;

    public CategoryForGetFiltersQueryResult(int id, string name)
    {
        Id = id;
        Name = name;
    }
}