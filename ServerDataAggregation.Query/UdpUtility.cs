using System.Net.Sockets;
using System.Net;

namespace ServersDataAggregation.Query;

internal class UdpUtility : INetCommunicate
{
    private string _address;
    private int _port;

    private UdpClient _client;

    IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);

    internal UdpUtility(string pAddress, int pPort, int receiveTimeout = 1000, int sendTimeout = 1000)
    {
        _address = pAddress;
        _port = pPort;

        _client = new UdpClient(_address, _port);
        _client.Client.ReceiveTimeout = receiveTimeout;
        _client.Client.SendTimeout = sendTimeout;
    }

    public byte[] SendBytes(byte[] pSendBytes)
    {
        int i = _client.Send(pSendBytes, pSendBytes.Length);
        byte[] receiveBytes = _client.Receive(ref sender);
        return receiveBytes;            
    }

    public string RemoteIpAddress
    {
        get { return ((IPEndPoint)_client.Client.RemoteEndPoint).Address.ToString(); }
    }

    public int RemotePort
    {
        get { return ((IPEndPoint)_client.Client.RemoteEndPoint).Port; }
    }
}
