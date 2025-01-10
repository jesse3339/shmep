using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SHMedicalEquipmentProcessor.Application.Common.Interfaces;
using SHMedicalEquipmentProcessor.Infrastructure.Services;

namespace SHMedicalEquipmentProcessor.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IMedicalEquipmentOrderClient, MedicalEquipmentOrderClient>(options =>
        {
            options.BaseAddress = new Uri(configuration["MedicalEquipmentOrders:BaseUrl"] ?? throw new InvalidOperationException());
            // could also set default auth headers
        });

        services.AddHttpClient<IMedicalEquipmentAlertClient, MedicalEquipmentAlertClient>(options =>
        {
            options.BaseAddress = new Uri(configuration["OrderAlerts:BaseUrl"] ?? throw new InvalidOperationException());
        });

        services.AddHttpClient<IMedicalOrderUpdateClient, MedicalOrderUpdateClient>(options =>
        {
            options.BaseAddress =
                new Uri(configuration["MedicalOrderUpdates:BaseUrl"] ?? throw new InvalidOperationException());
        });
        
        
        return services;
    }
}