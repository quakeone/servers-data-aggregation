using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerDataAggregation.Persistence;
using ServersDataAggregation.Service.Tasks;
using ServersDataAggregation.Service.Tasks.QueryServers;

namespace ServersDataAggregation.Service.Scheduler;

public delegate void DispatchMessage(string pTaskName, string pMessage);
public delegate void DispatchException(string pTaskName, Exception pException);

public class ScheduleDispatcher : IHostedService
{
    public DispatchMessage MessageOut { get; set; }
    public DispatchException ExceptionOut { get; set; }
    private readonly ILogger _logger;

    // private Thread _thread;
    private bool _stop = true;
    private Timer _timer;
    private List<ScheduledTask> _scheduledtasks;

    public ScheduleDispatcher(
        ILogger<ScheduleDispatcher> logger,
        IHostApplicationLifetime appLifetime)
    {
        using (var context = new PersistenceContext())
        {
            // migrate any database changes on startup (includes initial db creation)
            context.Database.Migrate();
            context.Database.EnsureCreated();
        }
        _logger = logger;

        var synchronizeServers = new SynchronizeServers();

        var sycnrhonizeServersTask = new ScheduledTask(
              synchronizeServers.Execute
            , "Servers Synchrnoize"
            , new IntervalSchedule(60, new IntervalScheduleOptions{RunImmediately = true}));

        var queryServers = new QueryServers();
        var queryTask = new ScheduledTask(
            queryServers.Execute
            , "Query Thread"
            , new IntervalSchedule(3));

        var cleanup = new Cleanup();
        var cleanupTask = new ScheduledTask(
            cleanup.Execute
            , "Cleanup"
            , new IntervalSchedule(60));

        _scheduledtasks = new List<ScheduledTask>()
        {
            cleanupTask,
            queryTask,
            sycnrhonizeServersTask
        };
    }

    public void AddTask(ScheduledTask pTask)
    {
        pTask.MessageOut += new DispatchMessage((task, message) => PublishMessage(task, message));
        _scheduledtasks.Add(pTask);
    }

    public bool IsRunning
    {
        get
        {
            return !_stop;
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _stop = false;
        _timer = new Timer(
            Run,
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(1)
        );
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _stop = true;
        return Task.CompletedTask;
    }

    public void Run(object state)
    {
        DateTime checkDate = DateTime.Now;

        for(int i = 0; i < _scheduledtasks.Count; i++)
        {
            ScheduledTask task = _scheduledtasks[i];

            if (!task.IsExecuting && task.NeedsExecution())
            {
                try
                {
                    Task.Run(task.Execute).ContinueWith(t =>
                    {
                        foreach(var ex in t.Exception.InnerExceptions)
                        {
                            PublishException(task.Name, ex);
                        }
                    }, TaskContinuationOptions.OnlyOnFaulted);
                }
                catch (Exception pException)
                {
                    PublishException(task.Name, pException);
                }
            }
        }
    }

    private void PublishMessage(string pTaskName, string pMessage)
    {
        if (MessageOut != null)
        {
            MessageOut.Invoke(pTaskName, pMessage);
        }
    }
    private void PublishException(string pTaskName, Exception pException)
    {
        if (ExceptionOut != null)
        {
            ExceptionOut.Invoke(pTaskName, pException);
        }
    }
}
