using Azure.Storage.Blobs;
using Imanys.SolenLms.Application.Resources.Core.UseCases;
using Imanys.SolenLms.Application.Resources.Core.UseCases.Lectures.Commands.UploadLectureVideo;
using Imanys.SolenLms.Application.Resources.Infrastructure.Storage.AzureBlobStorage;
using Imanys.SolenLms.Application.Resources.Infrastructure.Storage.Local;
using Imanys.SolenLms.Application.Resources.Infrastructure.Storage.Local.Videos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Imanys.SolenLms.Application.Resources.Infrastructure.DependencyInjection;

internal static class Storage
{
    internal static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {


        bool isBlobStorageEnabled = bool.Parse(configuration["AzureBlobStorageSettings:Enabled"] ?? "false");
        if (isBlobStorageEnabled)
            services.AddScoped<IMediaManager, AzureBobVideoManager>();
        else
            services.AddScoped<IMediaManager, LocalVideoManager>();


        services.Configure<LocalStorageSettings>(configuration.GetSection("LocalStorageSettings"));
        services.Configure<ResourcesSettings>(configuration.GetSection("ResourcesSettings"));

        services.AddSingleton(_ => new BlobServiceClient(configuration["AzureBlobStorageSettings:ConnectionString"]));
        services.Configure<AzureBlobStorageSettings>(configuration.GetSection("AzureBlobStorageSettings"));

        return services;
    }
}
