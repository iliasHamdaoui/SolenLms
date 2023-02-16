using Imanys.SolenLms.Application.Shared.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

WebApplication app = builder
    .ConfigureServices()
    .ConfigurePipeline(builder.Configuration);

app.Run();
