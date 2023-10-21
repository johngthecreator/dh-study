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
                .WithOpenAIChatCompletionService(Constants.OpenAiChatCompletionModel, apiKey)
                .WithOpenAITextEmbeddingGenerationService(Constants.OpenAiEmbeddingModel, apiKey)
                .WithMemoryStorage(new VolatileMemoryStore())
                .Build();
    }
}