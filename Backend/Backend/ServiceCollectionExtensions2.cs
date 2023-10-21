using Backend.Services;
using Backend.Services.DataService;

namespace Backend;

public static class ServiceCollectionExtensions2
{
    public static IServiceCollection AddServices2(this IServiceCollection services) {
    
        return 
            services

                .AddScoped<IDataService, DataService>()
                .AddScoped<TextEmbeddingService>()
                .AddScoped<IUserAuthService, DevUserAuthService>();
            
    
    }
}
