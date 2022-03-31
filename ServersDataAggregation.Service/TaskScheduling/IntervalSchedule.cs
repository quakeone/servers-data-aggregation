namespace ServersDataAggregation.Service.Scheduler;
public class IntervalSchedule : Schedule
{
    public int IntervalSeconds { get; private set; }
        
    public IntervalSchedule(int pIntervalSeconds)
    {
        LastRun = DateTime.UtcNow;
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
