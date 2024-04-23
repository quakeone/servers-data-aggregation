using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Query.Master
{
    public class QWMaster
    {
        const string QW_QUERY = "c\n";

        public QWMaster() { }

        internal static byte[] GetServersRequest()
        {
            var queryBytes = Encoding.UTF8.GetBytes(QW_QUERY);

            byte[] bytes = new byte[queryBytes.Length + 1];
            Buffer.BlockCopy(queryBytes, 0, bytes, 0, queryBytes.Length);

            bytes[queryBytes.Length] = 0xff;
            return bytes;
        }

        private bool IsEoT(byte[] bytes, int offset)
        {
            return bytes[offset] == 0x45
                && bytes[offset + 1] == 0x4f
                && bytes[offset + 2] == 0x54;
        }

        public ServerAddress? GetAddress(byte[] bytes, int offset)
        {
            var port = bytes[offset + 4] << 8 | bytes[offset + 5];
            return new ServerAddress
            {
                Address = $"{(int)bytes[offset++]}.{(int)bytes[offset++]}.{(int)bytes[offset++]}.{(int)bytes[offset++]}",
                Port = port,
                Type = AddressType.IPv4
            };
        }

        private ServerAddress[] ParseBytes(byte[] pBytes)
        {
            int byteCounter = 0;
            if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
            if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
            if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
            if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
            if (pBytes[byteCounter++] != 0x64) throw new Exception("bad bytes");
            if (pBytes[byteCounter++] != 0x0a) throw new Exception("bad bytes");

            var addresses = new List<ServerAddress>();

            for (;(pBytes.Length - byteCounter) > 6 && !IsEoT(pBytes, byteCounter); byteCounter += 6)
            {
                var newAddress = GetAddress(pBytes, byteCounter);
                if (newAddress != null)
                    addresses.Add(newAddress);
                
            }

            return addresses.ToArray();
        }

        public ServerAddress[] GetServers(string url, int port = 27000)
        {
            var udp = new UdpUtility(url, port, 5000, 5000);
            byte[] receivedBytes;
            var list = new List<ServerAddress>();
            try
            {
                receivedBytes = udp.SendBytes(GetServersRequest());

                if (receivedBytes == null || receivedBytes.Length == 0)
                    throw new SocketException((int)SocketError.NoData);

                list.AddRange(ParseBytes(receivedBytes));
              
            }
            catch (Exception e)
            {
                throw;
            }
            return list.ToArray();
        }
    }
}
