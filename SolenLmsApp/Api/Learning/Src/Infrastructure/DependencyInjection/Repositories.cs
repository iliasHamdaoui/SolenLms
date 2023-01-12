using Imanys.SolenLms.Application.Learning.Core.UseCases.LearnersProgress.Commands.UpdateLearnerProgress;
using Imanys.SolenLms.Application.Learning.Infrastructure.Data.Repositories.LearnersProgress;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Learning.Infrastructure.DependencyInjection;
internal static class Repositories
{
    internal static IServiceCollection AddRepositories(this IServiceCollection services)
    {

        services.AddScoped<IUpdateLearnerProgressRepo, UpdateLearnerProgressRepo>();

        return services;
    }
}
