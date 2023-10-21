using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Backend.Services;

public class EmbeddingCacheService
{
    private readonly IMemoryCache _memoryCache;

    public EmbeddingCacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public bool TryGetEmbeddings(string? userGuid, out ISemanticTextMemory embeddings)
    {
        return _memoryCache.TryGetValue(GetCacheKey(userGuid), out embeddings);
    }

    public void SetEmbeddings(string? userGuid, ISemanticTextMemory embeddings)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions
        {
            // Set your cache expiration preferences here
            SlidingExpiration = TimeSpan.FromMinutes(10)
        };

        _memoryCache.Set(GetCacheKey(userGuid), embeddings, cacheEntryOptions);
    }

    private string GetCacheKey(string? userGuid)
    {
        return $"Embeddings-{userGuid}";
    }
}