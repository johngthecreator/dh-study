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
        {
            KernelBuilder = new KernelBuilder()
                .WithOpenAIChatCompletionService(Constants.OpenAi32KContext, apiKey)
                .WithOpenAITextEmbeddingGenerationService(Constants.OpenAi32KContext, apiKey)
                .WithMemoryStorage(new VolatileMemoryStore())
                .Build();
        }


    }
}