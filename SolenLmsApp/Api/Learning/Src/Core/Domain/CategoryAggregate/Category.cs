﻿using Imanys.SolenLms.Application.Learning.Core.Domain.CourseAggregate;
using Imanys.SolenLms.Application.Shared.Core;

namespace Imanys.SolenLms.Application.Learning.Core.Domain.CategoryAggregate;

public sealed class Category : IAggregateRoot
{
    private readonly List<Course> _courses = new();

    public Category(string organizationId, int id, string name)
    {
        OrganizationId = organizationId;
        Id = id;
        Name = name;
    }

    public string OrganizationId { get; init; }
    public int Id { get; init; }
    public string Name { get; private set; }
    public IEnumerable<Course> Courses => _courses.AsReadOnly();

    public void UpdateName(string name)
    {
        Name = name;
    }
}