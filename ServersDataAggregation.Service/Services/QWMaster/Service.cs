using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Db = ServerDataAggregation.Persistence.Models;
using ServersDataAggregation.Query.Master;
using Microsoft.EntityFrameworkCore.Metadata;
using ServersDataAggregation.Common;
using System.Net;

namespace ServersDataAggregation.Service.Services.QWMaster;



public class Service
{
    public Service() { }

    public Db.Server[] GetQWServers()
    {
        var master = new Query.Master.QWMaster();
        var newServers = new ServerAddress[0];
        var url = "qwmaster.fodquake.net";

        try
        {
            newServers = master.GetServers(url);
        }
        catch (Exception ex)
        {
            Logging.LogWarning($"Error synchronizing with master server {url}: {ex.Message}");
        }

        return newServers.Select(address =>
            new Db.Server
            {
                GameId = (int)Game.QuakeWorld,
                Port = address.Port,
                Address = address.Address,
                QueryInterval = 20,
                Mod = "",
                Active = true,
                ApiKey = Guid.NewGuid().ToString(),
                Source = url
            }).ToArray();
    }
}
