using System.Collections;
using System.Text;

namespace ServersDataAggregation.Query.Games.Quake3.Packets;

public class Q3ServerStatus
{
    private const byte SLASH_DELIMITER = 0x5c;
    private const byte NEWLINE_DELIMITER = 0x0a;

    internal Hashtable ServerSettings;
    internal List<Q3PlayerStatus> CurrentPlayers;

    internal static byte[] StatusRequest
    {
        get
        {
            // 0xFFFFFFFFgetstatus\n
            return new byte[]{ 0xff, 0xff, 0xff, 0xff,
               0x67, 0x65, 0x74, 0x73, 0x74, 0x61, 0x74, 0x75, 0x73, 0x0a , 0x00 };
        }
    }

    internal Q3ServerStatus()
    {
        ServerSettings = new Hashtable();
        CurrentPlayers = new List<Q3PlayerStatus>();
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

            Q3PlayerStatus pStatus = new Q3PlayerStatus();
            pStatus.Frags = playerStatus[0];
            pStatus.Ping = playerStatus[1];
            pStatus.PlayerName = playerStatus[2];
            pStatus.PlayerNameBytes = Encoding.ASCII.GetBytes(pStatus.PlayerName); // TODO: take this *BEFORE* converting to string above
            
            CurrentPlayers.Add(pStatus);
        }  
    }
}
