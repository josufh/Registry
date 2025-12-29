namespace Registry.Services.Cache;

public static class Startup
{
    public static IServiceCollection AddBlobUploadCache(
        this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddScoped<IBlobUploadCache, BlobUploadCache>();
        
        return services;
    }
}