using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;

namespace ConsoleUI;

// DI (Dependency Injection), Serilog, Settings

public class Program
{
    static void Main(string[] args)
    {
        var builder = new ConfigurationBuilder();
        BuildConfig(builder);

        var dateTimeStr = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(builder.Build())
            .Enrich.FromLogContext()
            .WriteTo.Console()
            //.WriteTo.File($"C:\\temp\\IcptUploader_{dateTimeStr}.txt")
            //.WriteTo.File(path:$"C:\\temp\\IcptUploader_{dateTimeStr}.json", formatter: new JsonFormatter())
            .WriteTo.Seq("http://localhost:5341")
            .CreateLogger();

        Log.Logger.Information("Applicatoin Starting");

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                services.AddTransient<IGreetingService, GreetingService>();
            })
            .UseSerilog()
            .Build();

        var service = ActivatorUtilities.CreateInstance<GreetingService>(host.Services);
        service.Run();

        Log.CloseAndFlush();
    }

    static void BuildConfig(IConfigurationBuilder builder)
    {
        builder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables();
    }
}
