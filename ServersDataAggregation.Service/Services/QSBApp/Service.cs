using ServerDataAggregation.Persistence.Models;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ServersDataAggregation.Service.Services.QSBApp
{
    internal class Service
    {
        private Server Transform(QSBServer server)
        {
            return new Server
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
        public async Task<Server[]> GetServers()
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("https://servers.quakeone.com/api/servers/manage/server"))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var qsbServers = JsonSerializer.Deserialize<List<QSBServer>>(apiResponse);
                    return qsbServers.Select(Transform).ToArray();
                }
            }
        }
    }
}
