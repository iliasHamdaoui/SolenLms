using Imanys.SolenLms.Application.WebClient;
using Imanys.SolenLms.Application.WebClient.Shared;
using Imanys.SolenLms.Application.WebClient.Shared.DI;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Toolbelt.Blazor.Extensions.DependencyInjection;

WebAssemblyHostBuilder builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddWebClient(builder.HostEnvironment.BaseAddress);
builder.Services.Configure<VideoResourcesUrl>(options =>
    options.Value = $"{builder.HostEnvironment.BaseAddress}api/resources/lectures/video");


builder.UseLoadingBar();

await builder.Build().RunAsync();