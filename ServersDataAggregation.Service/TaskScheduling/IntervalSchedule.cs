namespace ServersDataAggregation.Service.Scheduler;
public class IntervalScheduleOptions 
{
    public bool RunImmediately { get; set; } = false;
}
public class IntervalSchedule : Schedule
{
    public int IntervalSeconds { get; private set; }
        
    public IntervalSchedule(int pIntervalSeconds, IntervalScheduleOptions? options = null)
    {
        LastRun = DateTime.UtcNow;
        if (options != null)
        {
            LastRun = options.RunImmediately ? DateTime.MinValue : LastRun;
        }
        IntervalSeconds = pIntervalSeconds;
    }

    public override bool NeedsExecution()
    {
        DateTime checkDate = DateTime.UtcNow;
        DateTime testDate = LastRun;
        if (checkDate > testDate.AddSeconds(IntervalSeconds))
        {
            return true;
        }

        return false;
    }
}
