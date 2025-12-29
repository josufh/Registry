namespace Registry.Middleware;

public static class Startup
{
    public static IApplicationBuilder UseBlobInterceptionMiddleware(
        this IApplicationBuilder app)
    {
        return app.UseMiddleware<BlobInterceptionMiddleware>();
    }
}