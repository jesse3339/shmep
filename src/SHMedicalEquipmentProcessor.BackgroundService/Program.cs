using SHMedicalEquipmentProcessor.BackgroundService;
using SHMedicalEquipmentProcessor.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();