using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Serilog;
using System.Security.Claims;
using WebClient.Startup.Middlewares;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()
    .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddAuthorization();
builder.Services.AddBff().AddRemoteApis();

builder.Services.AddRazorPages();


builder.Services.Configure<FormOptions>(x => { x.MultipartBodyLengthLimit = 2147483648; }); // 2 GB
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 2147483648;
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = "__Host-blazor";
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Events.OnSigningOut = async e => { await e.HttpContext.RevokeRefreshTokenAsync(); };
    })
    .AddOpenIdConnect("oidc", options =>
    {
        options.Authority = builder.Configuration["oidc:Authority"];
        options.ClientId = builder.Configuration["oidc:ClientId"];
        options.ClientSecret = builder.Configuration["oidc:ClientSecret"];
        options.ResponseType = "code";
        options.ResponseMode = "query";
        options.AccessDeniedPath = "/";
        // options.SignedOutRedirectUri  = "/signedout";
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("solenLmsApi");
        options.Scope.Add("solenLmsProfile");
        options.Scope.Add("IdentityServerApi");
        options.Scope.Add("offline_access");
        //options.Events.OnSignedOutCallbackRedirect += context =>
        //{
        //    context.Response.Redirect(context.Options.SignedOutRedirectUri);
        //    context.HandleResponse();

        //    return Task.CompletedTask;
        //};

        options.MapInboundClaims = false;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClaimActions.MapUniqueJsonKey("organizationId", "organizationId");
        options.ClaimActions.MapUniqueJsonKey(ClaimTypes.Role, ClaimTypes.Role);
        options.SaveTokens = true;
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAddXCSRFHeaderMiddlewareForResourcesApi();

app.UseBff();
app.UseAuthorization();

app.MapRazorPages();

app.MapBffManagementEndpoints();

app.MapRemoteBffApiEndpoint("/api", builder.Configuration["SolenApiEndpoint"]!)
    .RequireAccessToken();

app.MapRemoteBffApiEndpoint("/idp-api", $"{builder.Configuration["oidc:Authority"]}/idp-api")
    .RequireAccessToken();

app.MapFallbackToFile("index.html");


app.Run();