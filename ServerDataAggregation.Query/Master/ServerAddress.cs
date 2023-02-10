using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Query.Master;

public enum AddressType
{
    IPv4,
    IPv6
}
public class ServerAddress
{
    public string Address { get; set; }
    public int Port { get; set; }
    public AddressType Type { get; set; }
}
