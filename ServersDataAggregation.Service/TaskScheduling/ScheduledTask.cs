namespace ServersDataAggregation.Service.Scheduler;

public delegate void ScheduledTaskFn();

public class ScheduledTask
{
    public string Name { get; private set; }
    public Object TaskObject { get; private set; }
    public ScheduledTaskFn TaskMethod { get; private set; }
    public List<Schedule> Schedules { get; private set; }
    public bool IsExecuting { get; private set; }

    public DispatchMessage MessageOut { get; set; }

    // Default constructor
    public ScheduledTask(ScheduledTaskFn pTaskMethod, string pName, Schedule pInitialSchedule)
    {
        Name = pName;
        Schedules = new List<Schedule>();
        Schedules.Add(pInitialSchedule);

        TaskMethod = pTaskMethod;
    }

    public ScheduledTask(Object pTaskObject, ScheduledTaskFn pTaskMethod, string pName, Schedule pInitialSchedule)
        : this(pTaskMethod, pName, pInitialSchedule)
    {
        TaskObject = pTaskObject;
    }

    public bool NeedsExecution()
    {
        for (int i = 0; i < Schedules.Count; i++) 
            if (Schedules[i].NeedsExecution()) 
                return true; 

        return false;
    }

    public void Execute()
    {
        if (IsExecuting) return;
        IsExecuting = true;

        for (int i = 0; i < Schedules.Count; i++)
            Schedules[i].LastRun = DateTime.UtcNow;

        try
        {
            TaskMethod.Invoke();
            PublishMessage("Execution complete");  
        }
        catch(Exception ex)
        {
            PublishMessage("Error encountered executing: " + Name + Environment.NewLine + "Exception Detail: " + ex.ToString());  
        }
        finally
        {
            IsExecuting = false;
        }
         
    }

    private void PublishMessage(string pMessage)
    {
        if (MessageOut != null)
        {
            MessageOut.Invoke(this.Name, pMessage);
        }
    }
}