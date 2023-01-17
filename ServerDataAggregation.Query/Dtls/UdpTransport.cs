using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Tls;

namespace ServersDataAggregation.Query.Dtls;

public class UdpTransport : DatagramTransport
{
    private UdpClient _client;
    private IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

    public UdpTransport(string address, int port)
    {
        _client = new UdpClient(address, port);
        _client.Client.ReceiveTimeout = 5000;
        _client.Client.SendTimeout = 5000;
    }
    public void Close()
    {
        _client.Close();
    }

    public int GetReceiveLimit()
    {
        return 2048;
    }

    public int GetSendLimit()
    {
        return 2048;
    }

    public int Receive(byte[] buf, int off, int len, int waitMillis)
    {
        try
        {
            var bytes = _client.Receive(ref sender);

            bytes.CopyTo(buf, 0);

            return bytes.Length;
        }
        catch (SocketException ex)
        {
            // Timeouts are handled and proceeded by the lib
            // Catch here and throw an alert to get out.
            if (ex.SocketErrorCode == SocketError.TimedOut)
            {
                    throw new TlsFatalAlert(AlertDescription.internal_error);
            }
            throw;
        }
    }

    public void Send(byte[] buf, int off, int len)
    {
        _client.Send(buf, len);       
    }
}
