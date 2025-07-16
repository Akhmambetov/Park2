using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Park2.Application.Services.AttrationService;
using Park2.Domain.Models;
using Park2.Domain.Settings;
using Park2.Infrastructure;
using Park2.Presentation.ConsoleUI;
using Park2.Presentation.ConsoleUI.Extensions;


var configuration = new ConfigurationBuilder()
   .SetBasePath(Directory.GetCurrentDirectory())
   .AddJsonFile("appsettings.json", optional: false)
   .Build();

var services = new ServiceCollection();

services.AddLogging(builder =>
{
    builder.AddConsole(); // Чтобы видеть логи в консоли
    builder.SetMinimumLevel(LogLevel.Information);
});

services.AddTransient<AttractionProcessorService>();
services.AddSingleton<Park>();
services.AddSingleton<ParkInitialization>();

// Конфигурация SimulationSettings
services.Configure<SimulationSettings>(configuration.GetSection("SimulationSettings"));

// Регистрация всех сервисов симуляции
services.AddSimulationServices();

// Регистрация главного класса консольного приложения
services.AddSingleton<ConsoleApp>();

var serviceProvider = services.BuildServiceProvider();

var park = serviceProvider.GetRequiredService<Park>();
var initializer = serviceProvider.GetRequiredService<ParkInitialization>();
initializer.Initialize(park, configuration);

var app = serviceProvider.GetRequiredService<ConsoleApp>();
await app.Run();
