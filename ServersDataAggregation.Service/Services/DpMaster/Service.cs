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

namespace ServersDataAggregation.Service.Services.DpMaster;



public class Service
{
    public Service() { }

    public Db.Server[] GetDPServers()
    {
        var game = "DarkPlaces-Quake";
        var url = "dpmaster.deathmask.net";

        var master = new Query.Master.DpMaster();
        var newServers = master.GetServers(game, url);
        return newServers.Select(address =>
            new Db.Server
            {
                GameId = (int)Game.NetQuake,
                Port = address.Port,
                Address = address.Address,
                QueryInterval = 20,
                Mod = "",
                Active = true,
                ApiKey = Guid.NewGuid().ToString(),
                Parameters = @"{ ""Engine"": ""dp"" }",
                Source = url
            }).ToArray();
    }

    public Db.Server[] GetFTEServers()
    {
        var game = "FTE-Quake";
        var url = "master.frag-net.com";

        var master = new Query.Master.DpMaster();
        var newServers = master.GetServers(game, url);
        return newServers.Select(address =>
            new Db.Server
            {
                GameId = (int)Game.NetQuake,
                Port = address.Port,
                Address = address.Address,
                QueryInterval = 20,
                Mod = "",
                Active = true,
                ApiKey = Guid.NewGuid().ToString(),
                Parameters = @"{ ""Engine"": ""fte"" }",
                Source = url
            }).ToArray();
    }
}
