using ServersDataAggregation.Common;
using ServersDataAggregation.Service.Services.QSBApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServersDataAggregation.Service.Services.IpApi;

public class Service
{
    private const string URL = "http://ip-api.com/json/";

    public async Task<IpResult> GetResult(string address)
    {
        using (var httpClient = new HttpClient())
        {
            using (var response = await httpClient.GetAsync($"{URL}{address}"))
            {
                string apiResponse = await response.Content.ReadAsStringAsync();
                try
                {
                    return JsonSerializer.Deserialize<IpResult>(apiResponse);
                }
                catch (Exception ex)
                {
                    Logging.LogError(ex, $"Failed to parse feed from IP API for {address}");
                    return new IpResult { status = "fail" };
                }
            }
        }
    }
}
