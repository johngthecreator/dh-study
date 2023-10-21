using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Memory;

namespace Backend.Services;

public class KernelService
{
    public readonly IKernel KernelBuilder;
        
    public KernelService(IConfiguration configuration)
    {
        string apiKey = configuration.GetConnectionString("OpenAiApiKey");
        string gptModel = configuration.GetConnectionString("OpenAiGptModel");
        
        if (KernelBuilder == null)
        {
            KernelBuilder = new KernelBuilder()
                .WithOpenAIChatCompletionService(gptModel, apiKey)
                .WithOpenAITextEmbeddingGenerationService("text-embedding-ada-002", apiKey)
                .WithMemoryStorage(new VolatileMemoryStore())
                .Build();
        }


    }
}