using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using ServerDataAggregation.Persistence.Models;
using ServersDataAggregation.Common;
using ServersDataAggregation.Common.Model;
using System.Text.Json;
using Db = ServerDataAggregation.Persistence.Models;

namespace ServersDataAggregation.Service.Services.QSBApp
{
    internal class Service
    {
        private Db.Server Transform(QSBServer server)
        {
            return new Db.Server
            {
                GameId = server.GameId,
                Port = server.Port,
                Address = server.DNS,
                Locality = server.Location,
                QueryInterval = server.QueryInterval,
                Mod = server.ModificationCode,
                Active = server.Active == 1,
                Parameters = server.Parameters,
                ApiKey = server.ApiKey ?? new Guid().ToString()
            };
        }
        public async Task<Db.Server[]> GetServers()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://servers.quakeone.com/api/servers/manage/server"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    List<QSBServer> qsbServers;
                    try {
                        qsbServers = JsonSerializer.Deserialize<List<QSBServer>>(apiResponse);
                    } catch(Exception ex) {
                        Logging.LogError(ex, "Failed to parse feed from QSB Server");
                        return new Db.Server[0];
                    }
                    return qsbServers
                        .Select(Transform)
                        .Concat(new Db.Server[] {
                            new Db.Server()
                            {
                                GameId = 0,
                                Port = 26020,
                                Address = "sv.netquake.io",
                                Locality = "somewhere",
                                QueryInterval = 10,
                                Mod = "CRMOD",
                                Active = true,
                                ApiKey = new Guid().ToString()
                            }
                        })
                        .ToArray();
                }
            }
        }
    }
}
