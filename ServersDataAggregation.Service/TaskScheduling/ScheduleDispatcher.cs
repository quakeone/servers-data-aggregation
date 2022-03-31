using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
        _logger = logger;



        // Add Queries schedule task 
        QSB.Server.ServerQueryController queryController = new QSB.Server.ServerQueryController(
            pDataSessionFactory
            , new QSB.GameServerInterface.ServerInterface()
            );

        var queryTask = new ScheduledTask(
            queryController
            , new Task(queryController.DoQueries)
            , "Query Thread"
            , new IntervalSchedule(3));

        // For Reporting and Rollup tables - scheduled task(s)
        QSB.Server.ServerManager serverManager = new QSB.Server.ServerManager(pDataSessionFactory);

        var aggregatorTask = new ScheduledTask(
            serverManager
            , new QSB.Common.TaskScheduling.Task(serverManager.SavePreviousDaysHistoricalSummery)
            , "HistoricalHourlySummery"
            , new QSB.Common.TaskScheduling.TimeOfDaySchedule(new TimeSpan(0, 1, 0)));

        _scheduledtasks = new List<ScheduledTask>()
        {
            queryTask,
            aggregatorTask
        }
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
