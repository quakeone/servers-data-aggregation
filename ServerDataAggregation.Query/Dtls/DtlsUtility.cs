using Org.BouncyCastle.Tls;

namespace ServersDataAggregation.Query.Dtls;

public class DtlsUtility : INetCommunicate
{
    private byte[] _psk;
    private byte[] _pskId;
    private DatagramTransport dtlsTransport;

    public string RemoteIpAddress { get; set; }

    public int RemotePort { get; set; }

    public DtlsUtility(byte[] psk, byte[] pskId, string serverAddress, int port)
    {
        _psk = psk;
        _pskId = pskId;

        RemoteIpAddress = serverAddress;
        RemotePort = port;

        var client = new PskDtlsClient(new BasicTlsPskIdentity(_pskId, _psk));
        var transport = new UdpTransport(serverAddress, port);

        DtlsClientProtocol dtls = new DtlsClientProtocol();
        dtlsTransport = dtls.Connect(client, transport);
    }

    public void Send(byte[] tosend)
    {
        dtlsTransport.Send(tosend, 0, tosend.Length);
    }
    public byte[] SendBytes(byte[] tosend)
    {
        dtlsTransport.Send(tosend, 0, tosend.Length);
        return Receive();
    }

    public byte[] Receive()
    {
        var buffer = new byte[2048];
        var received = dtlsTransport.Receive(buffer, 0, 2048, 3000);
        if (received > 0)
        {
            return buffer.Take(received).ToArray();
        }
        return new byte[] { };
    }

}
