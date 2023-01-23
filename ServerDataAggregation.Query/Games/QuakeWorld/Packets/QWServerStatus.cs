using System.Text;
using System.Collections;
using ServersDataAggregation.Common.Model;

namespace ServersDataAggregation.Query.Games.QuakeWorld.Packets;

public class QWServerStatus : QWStatusPacketBase
{
    private const byte STATUS_N = 0x0a;
    private const byte DELIMITER_SPACE = 0x20;
    private const byte DELIMITER_QUOTE = 0x22;

    internal List<QWPlayerStatus> CurrentPlayers;


    internal QWServerStatus()
    {
        ServerSettings = new Hashtable();
        CurrentPlayers = new List<QWPlayerStatus>();
    }

    internal void ParseBytes(byte[] pBytes, ServerParameters parameters)
    {
        int byteCounter = 0;
        if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
        if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
        if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
        if (pBytes[byteCounter++] != 0xff) throw new Exception("bad bytes");
        if (pBytes[byteCounter++] != 0x6e) throw new Exception("bad bytes");
       
        while(SettingWalker(pBytes,byteCounter, out byteCounter)) {}
        byteCounter++;
        int tempOffset = 0;
        while ((tempOffset = NextNewLineIndex(pBytes, byteCounter)) != -1 && tempOffset != pBytes.Length)
        {
            // 2 10 38 100 \"looser\" \"tf_scout\" 4 4
             
            int playerLength = tempOffset - byteCounter;

            QWPlayerStatus playerStatus = new QWPlayerStatus();
            int colNum = 0;
            int playerOffset = byteCounter;

            while (playerOffset < tempOffset)
            {
                int length = 0;
                bool quote = false;
                do{
                    if (pBytes[playerOffset + length] == DELIMITER_QUOTE)
                        quote = !quote;
                    length++;
                } while (playerOffset + length != tempOffset && (pBytes[playerOffset + length] != DELIMITER_SPACE || quote));
                

                switch (colNum)
                {
                    case 0:
                        playerStatus.PlayerNumber = Int32.Parse(Encoding.ASCII.GetString(pBytes, playerOffset, length));
                        break;
                    case 1:
                        playerStatus.Frags = Encoding.ASCII.GetString(pBytes, playerOffset, length);
                        break;
                    case 2:
                        if (!string.IsNullOrEmpty(parameters.Engine) && parameters.Engine.ToLower() == "fte")
                        {
                            // this is broken in FTE.
                            playerStatus.PlayMins = null;
                        }
                        else
                        {
                            playerStatus.PlayMins = Encoding.ASCII.GetString(pBytes, playerOffset, length);
                        }
                        break;
                    case 3:
                        playerStatus.Ping = Encoding.ASCII.GetString(pBytes, playerOffset, length);
                        break;
                    case 4:
                        playerStatus.PlayerBytes = new byte[length-2];
                        Buffer.BlockCopy(pBytes, playerOffset+1, playerStatus.PlayerBytes, 0, length-2);
                        break;
                    case 5:
                        playerStatus.SkinName = Encoding.ASCII.GetString(pBytes, playerOffset, length);
                        break;
                    case 6:
                        playerStatus.ShirtColor = Encoding.ASCII.GetString(pBytes, playerOffset, length);
                        break;
                    case 7:
                        playerStatus.PantColor = Encoding.ASCII.GetString(pBytes, playerOffset, length);
                        break;
                }

                playerOffset += length + 1;
                colNum++;
            }

            byteCounter = tempOffset + 1;
            if(colNum > 3)
                CurrentPlayers.Add(playerStatus);
        }

        //for(int i = 1; i < strs.Length; i++)
        //{
        //    string[] playerStatus = strs[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
        //    if(playerStatus.Length != 8)
        //        continue;

        //    QWPlayerStatus pStatus = new QWPlayerStatus();
        //    pStatus.PlayerNumber = playerStatus[0];
        //    pStatus.Frags = playerStatus[1];
        //    pStatus.PlayMins = playerStatus[2];
        //    pStatus.Ping = playerStatus[3];
        //    pStatus.PlayerName = playerStatus[4];
        //    pStatus.SkinName = playerStatus[5];
        //    pStatus.ShirtColor = playerStatus[6];
        //    pStatus.PantColor = playerStatus[7];
            
        //}  

    }

    //private string GetValueUntilSlash(byte[] pBytes, int pOffset)
    //{
    //    int length = 0;
    //    int counter = pOffset;

    //    while (pBytes[counter] != SLASH_DELIMITER 
    //        && pBytes[counter] != NEWLINE_DELIMITER) { counter++; length++; }

    //    counter = pOffset;
    //    char[] str = new char[length];
    //    for(int i = 0; i < length; i++, counter++)
    //    {
    //        str[i] = (char)pBytes[counter];
    //    }

    //    return new string(str);
    //}
}
