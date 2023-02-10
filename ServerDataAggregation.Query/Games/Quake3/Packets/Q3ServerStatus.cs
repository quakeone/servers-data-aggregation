using ServersDataAggregation.Query.Games.Common;
using ServersDataAggregation.Query.Games.QuakeWorld.Packets;
using System.Collections;
using System.Text;

namespace ServersDataAggregation.Query.Games.Quake3.Packets;

public class Q3ServerStatus
{
    private const byte SLASH_DELIMITER = 0x5c;
    private const byte NEWLINE_DELIMITER = 0x0a;
    private const byte DELIMITER_SPACE = 0x20;
    private const byte DELIMITER_QUOTE = 0x22;

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
        if (!StatusParser.ValidateResponse(pBytes, "statusResponse", out byteCounter))
        {
            throw new Exception($"Invalid response");
        }

        foreach(var setting in StatusParser.GetSettings(pBytes, byteCounter+1, out byteCounter))
        {
            ServerSettings.Add(setting.Key, setting.Value);
        }

        byteCounter++;
        int tempOffset = 0;
        while ((tempOffset = StatusParser.NextNewLineIndex(pBytes, byteCounter)) != -1 && tempOffset != pBytes.Length)
        {
            // 2 10 38 100 \"looser\" \"tf_scout\" 4 4

            int playerLength = tempOffset - byteCounter;

            Q3PlayerStatus playerStatus = new Q3PlayerStatus();
            int colNum = 0;
            int playerOffset = byteCounter;

            while (playerOffset < tempOffset)
            {
                int length = 0;
                bool quote = false;
                do
                {
                    if (pBytes[playerOffset + length] == DELIMITER_QUOTE)
                        quote = !quote;
                    length++;
                } while (playerOffset + length != tempOffset && (pBytes[playerOffset + length] != DELIMITER_SPACE || quote));


                switch (colNum)
                {
                    case 0:
                        playerStatus.Frags = Encoding.UTF8.GetString(pBytes, playerOffset, length);
                        break;
                    case 1:
                        playerStatus.Ping = Encoding.UTF8.GetString(pBytes, playerOffset, length);
                        break;
                    case 2:
                        playerStatus.PlayerNameBytes = new byte[length - 2];
                        Buffer.BlockCopy(pBytes, playerOffset + 1, playerStatus.PlayerNameBytes, 0, length - 2);
                        break;
                }

                playerOffset += length + 1;
                colNum++;
            }

            byteCounter = tempOffset + 1;
            if (colNum > 2)
                CurrentPlayers.Add(playerStatus);
        }



        //for (int i = 2; i < strs.Length; i++)
        //{
        //    string[] playerStatus = strs[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
        //    if(playerStatus.Length != 3)
        //        continue;

        //    Q3PlayerStatus pStatus = new Q3PlayerStatus();
        //    pStatus.Frags = playerStatus[0];
        //    pStatus.Ping = playerStatus[1];
        //    pStatus.PlayerName = playerStatus[2];
        //    pStatus.PlayerNameBytes = Encoding.ASCII.GetBytes(pStatus.PlayerName); // TODO: take this *BEFORE* converting to string above
            
        //    CurrentPlayers.Add(pStatus);
        //}  
    }
}
