using Microsoft.Extensions.Caching.Memory;

namespace Registry.Services.Cache;

public sealed class BlobUploadCache : IBlobUploadCache
{
    private readonly IMemoryCache _cache;
    private readonly MemoryCacheEntryOptions _options;

    public BlobUploadCache(
        IMemoryCache cache)
    {
        _cache = cache;
        _options = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
    }
    
    public void NewUpload<T>(string key, T value)
    {
        _cache.Set(key, value, _options);
    }
}