using Registry.Models;

namespace Registry.Middleware;

public sealed class BlobInterceptionMiddleware
{
    private readonly RequestDelegate _next;

    public BlobInterceptionMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, Blob blob)
    {
        if (!IsBlobUploadRequest(context))
        {
            await _next(context);
            return;    
        }

        blob = new(context.Request.Body);

        await _next(context);
    }

    private static bool IsBlobUploadRequest(
        HttpContext context)
    {
        if (!(HttpMethods.IsPost(context.Request.Method) ||
              HttpMethods.IsPatch(context.Request.Method) || 
              HttpMethods.IsPut(context.Request.Method)))
        {
            return false;
        }

        string path = context.Request.Path.Value ?? string.Empty;

        return path.StartsWith("/v2/", StringComparison.Ordinal) &&
               path.Contains("/blobs/uploads", StringComparison.Ordinal);
    }
}