using App.Extensions;
using App.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace App;

public static class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        var service = host.Services.GetRequiredService<IAzureQueueService>();
        await service.RunAsync();
        Console.WriteLine("Press any key to exit !");
        Console.ReadKey();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((_, config) =>
            {
                config.AddJsonFile();
                config.AddUserSecrets();
                config.AddEnvironmentVariables();
                config.AddCommandLine(args);
            })
            .ConfigureLogging((hostingContext, loggingBuilder) =>
            {
                loggingBuilder.AddLogging(hostingContext);
            })
            .ConfigureServices((hostingContext, services) =>
            {
                services.AddTransient<IAzureQueueClient, AzureQueueClient>();
                services.AddTransient<IAzureQueueService, AzureQueueService>();
                services.Configure<Settings>(hostingContext.Configuration.GetSection("Settings"));
            })
            .UseConsoleLifetime();
}