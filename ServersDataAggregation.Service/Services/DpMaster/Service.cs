using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Service.Services.DpMaster
{
    public class Service
    {
        public Service() { }

        public void TryIt ()
        {
            var udpClient = new UdpClient();
           // tcpClient.Connect(new System.Net.IPAddress("dpmaster.deathmask.net", 27950));
        }
    }
}
