using Microsoft.Extensions.Caching.Memory;
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


    private string GetCacheKey(string? userGuid)
    {
        return $"Embeddings-{userGuid}";
    }
}