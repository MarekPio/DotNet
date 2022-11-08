using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;

namespace ConsoleUI;

public class GreetingService : IGreetingService
{
    private readonly ILogger<GreetingService> _log;
    private readonly IConfiguration _config;

    public GreetingService(ILogger<GreetingService> log, IConfiguration config)
    {
        _log = log;
        _config = config;
    }

    public void Run()
    {
        try
        {
            for (int i = 0; i < _config.GetValue<int>("LoopTimes"); i++)
            {
                if (i == 5)
                {
                    throw new Exception("This is our demo exception");
                }
                else
                {
                    _log.LogInformation("Run number {runNumber}", i);
                }

            }
        }
        catch(Exception ex)
        {
            _log.LogError(ex.Message);
        }
    }
}