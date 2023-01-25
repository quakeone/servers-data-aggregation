using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServersDataAggregation.Common;
using ServersDataAggregation.Service.Scheduler;

public class Program
{
    public static void Main(string[] args)
    {
        Logging.Initialize();
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ScheduleDispatcher>();
            });
}