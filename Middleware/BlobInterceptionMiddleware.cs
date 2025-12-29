namespace Registry.Middleware;

public sealed class BlobInterceptionMiddleware
{
    private readonly RequestDelegate _next;

    public BlobInterceptionMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        

        await _next(context);
    }
}