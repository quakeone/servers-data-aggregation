using System.Text;
using System.Collections;

namespace ServersDataAggregation.Query.Games.Quake2.Packets;

public class Q2ServerStatus
{
    private const byte SLASH_DELIMITER = 0x5c;
    private const byte NEWLINE_DELIMITER = 0x0a;

    internal Hashtable ServerSettings;
    internal List<Q2PlayerStatus> CurrentPlayers;

    internal static byte[] StatusRequest
    {
        get
        {
            // 0xFFFFFFFFstatus\n
            return new byte[] { 0xff, 0xff, 0xff, 0xff,
                0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x0a , 0x00 };
        }
    }

    internal Q2ServerStatus()
    {
        ServerSettings = new Hashtable();
        CurrentPlayers = new List<Q2PlayerStatus>();
    }

    internal void ParseBytes(byte[] pBytes)
    {
        int byteCounter = 0;
        if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
        if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
        if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
        if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");

        string str = Encoding.ASCII.GetString(pBytes, byteCounter, pBytes.Length - byteCounter);
        string[] strs = str.Split(new char[] { '\n' },StringSplitOptions.RemoveEmptyEntries);

        if (strs.Length < 2)
            throw new Exception("Invalid length");

        string[] settings = strs[1].Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < settings.Length - 1; i += 2)
        {
            ServerSettings.Add(settings[i], settings[i+1]);                
        }

        for(int i = 2; i < strs.Length; i++)
        {
            string[] playerStatus = strs[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            if(playerStatus.Length != 3)
                continue;

            Q2PlayerStatus pStatus = new Q2PlayerStatus();
            pStatus.Frags = playerStatus[0];
            pStatus.Ping = playerStatus[1];
            pStatus.PlayerName = playerStatus[2];
            
            CurrentPlayers.Add(pStatus);
        }  
    }
}
