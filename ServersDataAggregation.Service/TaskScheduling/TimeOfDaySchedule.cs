namespace ServersDataAggregation.Service.Scheduler;
public class TimeOfDaySchedule : Schedule
{
    public TimeSpan ExecuteTimeOfDay { get; private set; }

    public TimeOfDaySchedule(TimeSpan pTimeOfDay)
    {
        LastRun = DateTime.UtcNow.AddDays(-1);
        ExecuteTimeOfDay = pTimeOfDay;
    }

    public override bool NeedsExecution()
    {
        DateTime checkDate = DateTime.UtcNow;

        if (LastRun.Date < checkDate.Date
            && checkDate.TimeOfDay > ExecuteTimeOfDay)
        {
            return true;
        }

        return false;
    }
}