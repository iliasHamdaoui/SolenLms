using Imanys.SolenLms.Application.Learning.Features.LearnersProgress.Commands.UpdateLearnerProgress;
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
