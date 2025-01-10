using SHMedicalEquipmentProcessor.Application;
using SHMedicalEquipmentProcessor.BackgroundService;
using SHMedicalEquipmentProcessor.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddHostedService<OrderProcessorWorker>();

var host = builder.Build();
host.Run();