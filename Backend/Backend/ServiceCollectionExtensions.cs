using Backend.AzureBlobStorage;

namespace Backend;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        return 
            services.AddScoped<UploadAzure>();
    }
}