namespace ServersDataAggregation.Service.Scheduler;
public abstract class Schedule
{
    internal DateTime LastRun { get; set; }

    public abstract bool NeedsExecution();
}
