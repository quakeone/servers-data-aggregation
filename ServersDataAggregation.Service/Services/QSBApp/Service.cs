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
                ApiKey = server.ApiKey ?? new Guid().ToString(),
                Source = "QSB"
            };
        }
        public async Task<Db.Server[]> GetServers()
        {
            return new Db.Server[0];
            // This is obsolete.  Was used for testing.  Keeping as a template for future master server pulls.
            //using (var httpClient = new HttpClient())
            //{
            //    using (var response = await httpClient.GetAsync("https://servers.quakeone.com/api/servers/manage/server"))
            //    {
            //        string apiResponse = await response.Content.ReadAsStringAsync();
            // List<Db.Server> qsbServers = new []
            // try
            // {
            //     qsbServers = JsonSerializer.Deserialize<List<Db.Server>>(DUMP);
            // }
            // catch (Exception ex)
            // {
            //     Logging.LogError(ex, "Failed to parse feed from QSB Server");
            //     return new Db.Server[0];
            // }
            // return qsbServers
            //     .Select(s =>
            //     {
            //         s.ServerId = 0;
            //         return s;
            //     })
            //     .ToArray();
            //    }
            //}
        }
    }
}
