using Microsoft.Extensions.Logging;
using ServerDataAggregation.Persistence.Models;
using System.Text.Json;

namespace ServersDataAggregation.Service.Services.QSBApp
{
    internal class Service
    {
        private ILogger<Services.QSBApp.Service> _logger;
        public Service(){
            _logger = LoggerFactory.Create(options => {}).CreateLogger<Services.QSBApp.Service>();
        }
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
                    List<QSBServer> qsbServers;
                    try {
                        qsbServers = JsonSerializer.Deserialize<List<QSBServer>>(apiResponse);
                    } catch(Exception ex) {
                        _logger.Log(LogLevel.Error, ex, "Failed to parse feed from QSB Server");
                        return new Server[0];
                    }
                    return qsbServers.Select(Transform).ToArray();
                }
            }
        }
    }
}
