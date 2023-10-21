using Backend.AzureBlobStorage;
using Backend.Services;
using Backend.Services.AiServices;

namespace Backend;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<KernelService>();
        services.AddMemoryCache();
        services.AddScoped<EmbeddingCacheService>();
        services.AddScoped<ChatAiService>();
        services.AddScoped<FlashcardService>();
        services.AddScoped<MultipleChoiceService>();
        services.AddScoped<UploadAzure>();
        return services;
    }
}