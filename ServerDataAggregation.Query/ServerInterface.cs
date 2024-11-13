using Org.BouncyCastle.Crypto.Tls;
using ServersDataAggregation.Common;
using ServersDataAggregation.Common.Model;
using System.Net.Sockets;
using System.Text.Json;

namespace ServersDataAggregation.Query;

public class ServerInterface
{
    private static ServerParameters GetParameters(string parameters)
    {
        if (string.IsNullOrEmpty(parameters)) { return new ServerParameters(); }
        try
        {
            return JsonSerializer.Deserialize<ServerParameters>(parameters) ?? new ServerParameters();
        }
        catch (Exception ex)
        {
            return new ServerParameters();
        }
    }

    /// <summary>
    /// Call out to server and retreive information 
    /// </summary>
    /// <param name="pServerAddress">IP address or DNS of server</param>
    /// <param name="pServerPort">TCP/IP port to communicate on</param>
    /// <param name="pGame">Which game type server runs</param>
    /// <returns>ServerInfo object containing gathered informatino</returns>
    public static ServerSnapshot GetServerInfo(string pServerAddress, int pServerPort, Game pGame, string pParameters)
    {
        var parameters = GetParameters(pParameters);

        IServerInfoProvider? infoProvider = null;

        switch(parameters.Engine)
        {
            //case "fte":
            //    infoProvider = new Games.QuakeWorld.QuakeWorld(parameters);
            //    break;
            case "dp":
                infoProvider = new Games.Quake3.Quake3();
                break;
        }
        if (infoProvider == null)
        {
            switch (pGame)
            {
                case Game.NetQuake:
                    infoProvider = new Games.NetQuake.NetQuake();
                    break;
                case Game.QuakeWorld:
                    infoProvider = new Games.QuakeWorld.QuakeWorld(parameters);
                    break;
                case Game.Quake2:
                    infoProvider = new Games.Quake2.Quake2();
                    break;
                case Game.Quake3:
                    infoProvider = new Games.Quake3.Quake3();
                    break;
                case Game.QuakeEnhanced:
                    infoProvider = new Games.QuakeEnhanced.QuakeEnhanced();
                    break;

            }
        }
        
        if (infoProvider == null)
            throw new NotSupportedException("Game is not supported at this time");

        try
        {
            return infoProvider.GetServerInfo(pServerAddress, pServerPort);
        }
        catch (SocketException ex)
        {
            if (ex.SocketErrorCode == SocketError.HostNotFound)
                throw new ServerNotFoundException(ex);
            else //ex.SocketErrorCode == SocketError.TimedOut
                throw new ServerNotRespondingException(ex);
        }
        catch (TlsFatalAlert ex)
        {
            throw new ServerNotRespondingException(ex);
        }
        catch (Exception ex)
        {
            throw new ServerQueryParseException(ex);
        }
    }
}