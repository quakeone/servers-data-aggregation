using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Query.Master
{
    //https://github.com/fte-team/fteqw/blob/783185cc594e24d4f812ac6b289a09120db37265/plugins/serverb/net_master.c
    //https://github.com/kphillisjr/dpmaster/blob/master/doc/techinfo.txt
    //https://dpmaster.deathmask.net/
    public class DpMaster
    {
        const string DP_QUERY = "getserversExt {0} 3 empty full ipv4 ipv6\n";

        public DpMaster() { }

        internal static byte[] GetServersRequest(string game)
        {
            var query = string.Format(DP_QUERY, game);
            var queryBytes = Encoding.UTF8.GetBytes(query);

            byte[] bytes = new byte[queryBytes.Length + 4];
            Buffer.BlockCopy(queryBytes, 0, bytes, 4, queryBytes.Length);
            bytes[0] = 0xff;
            bytes[1] = 0xff;
            bytes[2] = 0xff;
            bytes[3] = 0xff;
            return bytes;
        }

        private bool IsEoT(byte[] bytes, int offset)
        {
            return bytes[offset] == 0x45
                && bytes[offset + 1] == 0x4f
                && bytes[offset + 2] == 0x54;
        }

        public ServerAddress? GetAddress(byte[] bytes, int offset, out int newOffset)
        {
            newOffset = offset;
            if (bytes[offset++] == '\\') // ipv4
            {
                newOffset = offset + 6;
                var port = bytes[offset + 4] << 8 | bytes[offset + 5];
                return new ServerAddress
                {
                    Address = $"{(int)bytes[offset++]}.{(int)bytes[offset++]}.{(int)bytes[offset++]}.{(int)bytes[offset++]}",
                    Port = port,
                    Type = AddressType.IPv4
                };
            } 
            else if (bytes[offset++] == '/') //ipv6
            {
                newOffset = offset += 18;
                // var port = bytes[4] << 4 | bytes[5];
                //for()
                //return new ServerAddress
                //{
                //    Address = $"{(int)bytes[0]}.{(int)bytes[1]}.{(int)bytes[2]}.{(int)bytes[3]}",
                //    Port = port,
                //    Type = AddressType.IPv4
                //};
            }
            return null;
        }

        private ServerAddress[] ParseBytes(byte[] pBytes)
        {
            int byteCounter = 0;
            if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
            if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
            if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
            if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");

            // getserversExtResponse
            string str = Encoding.ASCII.GetString(pBytes, byteCounter, 21);
            if (str != "getserversExtResponse")
            {
                throw new Exception($"Invalid response, expected getserversExtResponse got {str}");
            }

            byteCounter += 21;
            var addresses = new List<ServerAddress>();

            while((pBytes.Length - byteCounter) > 6 && !IsEoT(pBytes, byteCounter))
            {
                var newAddress = GetAddress(pBytes, byteCounter, out byteCounter);
                if (newAddress != null)
                    addresses.Add(newAddress);
            }

            return addresses.ToArray();
        }

        public ServerAddress[] GetServers( string game, string url, int port=27950)
        {
            var udp = new UdpUtility(url, port, 5000, 5000);
            byte[] receivedBytes;
            var list = new List<ServerAddress>();
            try
            {
                do
                {
                    receivedBytes = udp.SendBytes(GetServersRequest(game));
                
                    if (receivedBytes == null || receivedBytes.Length == 0)
                        throw new SocketException((int)SocketError.NoData);

                    list.AddRange(ParseBytes(receivedBytes));
                } while (!IsEoT(receivedBytes, receivedBytes.Length - 6) && !IsEoT(receivedBytes, receivedBytes.Length - 3));
            }
            catch (Exception e)
            {
                throw;
            }
            return list.ToArray();
        }
    }
}
