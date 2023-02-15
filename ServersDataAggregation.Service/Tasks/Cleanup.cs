using Microsoft.EntityFrameworkCore;
using ServerDataAggregation.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Service.Tasks;

public class Cleanup
{
    public async Task CleanSnapshotTable(PersistenceContext context)
    {
        var threshold = DateTime.UtcNow.AddDays(-3);
        var items = context.ServerSnapshots.Where(ss => ss.TimeStamp < threshold);
        context.ServerSnapshots.RemoveRange(items);
        context.SaveChanges();
    }
    public async Task Execute()
    {
        using (var context = new PersistenceContext())
        {
            await CleanSnapshotTable(context);
        }
    }
}
