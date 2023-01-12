namespace WebClient.Startup.Middlewares;

public class AddXCSRFHeaderMiddlewareForResourcesApi
{
    private readonly RequestDelegate _next;

    public AddXCSRFHeaderMiddlewareForResourcesApi(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        string path = context.Request.Path.ToString();

        if (path.StartsWith("/api/resources/lectures/video"))
        {
            IHeaderDictionary headers = context.Request.Headers;
            headers["X-CSRF"] = "1";
        }

        await _next(context);
    }
}

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseAddXCSRFHeaderMiddlewareForResourcesApi(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AddXCSRFHeaderMiddlewareForResourcesApi>();
    }
}




