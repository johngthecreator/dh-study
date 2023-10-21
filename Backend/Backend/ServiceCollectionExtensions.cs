using Backend.AzureBlobStorage;
using Backend.Services;

namespace Backend;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<KernelService>();
        services.AddScoped<UploadAzure>();
        return services;
    }
}