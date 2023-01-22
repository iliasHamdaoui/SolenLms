using AutoFixture;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateCourse;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateLecture;
using Imanys.SolenLms.Application.CourseManagement.Core.UseCases.Courses.Commands.CreateModule;
using Imanys.SolenLms.Application.CourseManagement.Infrastructure.Data;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data;
using Imanys.SolenLms.Application.Resources.Core.UseCases;
using Imanys.SolenLms.Application.Resources.Infrastructure.Data;
using Imanys.SolenLms.Application.Resources.Infrastructure.Storage.Local.Videos;
using Imanys.SolenLms.Application.Shared.Core.Events;
using Imanys.SolenLms.Application.Shared.Core.Infrastructure;
using Imanys.SolenLms.Application.Shared.Tests.Implementations;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Xunit;
using IAssemblyReference = Imanys.SolenLms.Application.WebApi.IAssemblyReference;

namespace Imanys.SolenLms.Application.Shared.Tests;

public class CustomSharedWebApplicationFactory : WebApplicationFactory<IAssemblyReference>, IAsyncLifetime
{
    private readonly TestcontainerDatabase _container;
    protected const string TestResourcesFolder = "TestResources";

    private readonly Fixture _fixture = new();
    private CreateCourseCommand _validCreateCourseCommand;
    private CreateModuleCommand _validCreateModuleCommand;
    private CreateLectureCommand _validCreateLectureCommand;
    public readonly Uri ResourcesBaseUrl = new("https://localhost/api/resources/lectures/");
    public readonly Uri InstructorCoursesBaseUrl = new("https://localhost/api/course-management/courses/");
    public readonly Uri LearnerCoursesBaseUrl = new("https://localhost/api/courses/");

    public Mock<IDateTime> DateTimeMoq { get; private set; }
    public IIntegrationEventsSender IntegrationEventsSender { get; protected set; }

    public CustomSharedWebApplicationFactory()
    {
        _container = new TestcontainersBuilder<MsSqlTestcontainer>()
            .WithDatabase(new MsSqlTestcontainerConfiguration { Password = "localdevpassword#123", })
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithCleanUp(true)
            .Build();

        DateTimeMoq = new Mock<IDateTime>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });

        _ = builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService)); // disable background services

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new(_container.ConnectionString)
            {
                TrustServerCertificate = true
            };

            services.RemoveDbContext<CourseManagementDbContext>();
            services.AddDbContext<CourseManagementDbContext>(options =>
            {
                options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString);
            });
            services.EnsureDbCreated<CourseManagementDbContext>();

            services.RemoveDbContext<ResourcesDbContext>();
            services.AddDbContext<ResourcesDbContext>(options =>
            {
                options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString);
            });
            services.EnsureDbCreated<ResourcesDbContext>();

            services.RemoveDbContext<LearningDbContext>();
            services.AddDbContext<LearningDbContext>(options =>
            {
                options.UseSqlServer(sqlConnectionStringBuilder.ConnectionString);
            });
            services.EnsureDbCreated<LearningDbContext>();

            services.RemoveAll<IDateTime>();
            services.AddTransient(_ => DateTimeMoq.Object);

            services.RemoveAll<IIntegrationEventsSender>();
            services.AddScoped<IIntegrationEventsSender, IntegrationEventsSender>();

            services.RemoveAll<IMediaManager>();
            services.AddScoped<IMediaManager, LocalVideoManager>();

            ConfigureServices(services);

            ServiceProvider sp = services.BuildServiceProvider();

            IntegrationEventsSender = sp.GetRequiredService<IIntegrationEventsSender>();
        });
    }

    public virtual void ConfigureServices(IServiceCollection services)
    {
    }

    public CreateCourseCommand GetValidCourseCreationCommand()
    {
        _validCreateCourseCommand ??= _fixture.Build<CreateCourseCommand>().Create();

        return _validCreateCourseCommand;
    }

    public CreateModuleCommand GetValidCreateModuleCommand()
    {
        _validCreateModuleCommand ??= _fixture.Build<CreateModuleCommand>().Create();

        return _validCreateModuleCommand;
    }

    public CreateLectureCommand GetValidCreateLectureCommand(string lectureTypeValue)
    {
        _validCreateLectureCommand ??= _fixture.Build<CreateLectureCommand>().Create();

        _validCreateLectureCommand.LectureType = lectureTypeValue;

        return _validCreateLectureCommand;
    }

    public async Task InitializeAsync() => await _container.StartAsync();

    public new async Task DisposeAsync()
    {
        try
        {
            var fileFolder = Path.Combine(Directory.GetCurrentDirectory(), TestResourcesFolder);
            if (Directory.Exists(fileFolder))
                Directory.Delete(fileFolder, true);
        }
        finally
        {
            await _container.DisposeAsync();
        }
    }
}