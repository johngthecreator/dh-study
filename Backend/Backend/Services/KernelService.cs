using Microsoft.Extensions.Caching.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Backend.Services;

public class KernelService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;

    public KernelService(IMemoryCache memoryCache, IConfiguration configuration)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
    }

    public IKernel GetKernel(string userId)
    {
        string apiKey = _configuration.GetConnectionString("OpenAiApiKey");

        return _memoryCache.GetOrCreate($"kernel_{userId}", entry =>
        {
            entry.SetSlidingExpiration(TimeSpan.FromMinutes(10));

            return new KernelBuilder()
                .WithOpenAIChatCompletionService("gpt-3.5-turbo-16k", apiKey)
                .WithOpenAITextEmbeddingGenerationService("text-embedding-ada-002", apiKey)
                .WithMemoryStorage(new VolatileMemoryStore())
                .Build();
        });
    }
}