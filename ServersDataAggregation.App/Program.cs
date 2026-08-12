using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServersDataAggregation.Common;
using ServersDataAggregation.Service.Scheduler;

public class Program
{
    public static void Main(string[] args)
    {
        DotEnv.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
        Logging.Initialize();

        // Queries block a pool thread on UDP receive; pre-seed rather than wait out
        // the pool's ~1-2 threads/sec ramp.
        ThreadPool.GetMinThreads(out _, out int completionPortThreads);
        ThreadPool.SetMinThreads(320, completionPortThreads);

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<ScheduleDispatcher>();
            });
}