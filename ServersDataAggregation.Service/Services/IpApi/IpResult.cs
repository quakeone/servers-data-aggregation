using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Service.Services.IpApi
{
    public class IpResult
    {
        public string? query { get; set; }
        public string status { get; set; }

        public string? countryCode { get; set; }
        public string? regionName { get; set; }
    }
}
