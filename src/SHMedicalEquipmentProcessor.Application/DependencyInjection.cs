using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SHMedicalEquipmentProcessor.Application.Features;

namespace SHMedicalEquipmentProcessor.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<OrderProcessor>();
        
        return services;
    }
}