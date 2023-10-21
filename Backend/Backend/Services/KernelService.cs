using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Backend.Services;

public class KernelService
{
    public readonly IKernel KernelBuilder;

    public KernelService(IConfiguration configuration)
    {
        string apiKey = configuration.GetConnectionString("OpenAiApiKey");

        if (KernelBuilder == null)
            KernelBuilder = new KernelBuilder()
                .WithOpenAIChatCompletionService("gpt-3.5-turbo-16k", apiKey)
                .WithOpenAITextEmbeddingGenerationService("text-embedding-ada-002", apiKey)
                .WithMemoryStorage(new VolatileMemoryStore())
                .Build();
    }
}