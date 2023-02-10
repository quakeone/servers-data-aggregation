using Org.BouncyCastle.Utilities;
using ServersDataAggregation.Common.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Query.Games.Common
{
    public class StatusParser
    {
        private const byte SLASH_DELIMITER = 0x5c;
        private const byte NEWLINE_DELIMITER = 0x0a;

        public static bool ValidateResponse(byte[] bytes, string responseString, out int length)
        {
            length = 4 + responseString.Length;

            if (bytes.Length > length)
            {
                if (bytes[0] == 0xff && bytes[1] == 0xff && bytes[2] == 0xff && bytes[3] == 0xff)
                {
                    // getserversExtResponse
                    string str = Encoding.ASCII.GetString(bytes, 4, responseString.Length);
                    if (str == responseString)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static int NextNewLineIndex(byte[] pBytes, int pStartIndex)
        {
            if (pStartIndex >= pBytes.Length)
                return -1;

            for (int i = pStartIndex; i < pBytes.Length; i++)
            {
                if (pBytes[i] == NEWLINE_DELIMITER)
                    return i;
            }
            return -1;
        }

        public static bool PlayerWalker(byte[] pBytes, int pOffset, out byte[] pPlayerBytes)
        {
            int i = pOffset;
            int tempOffset;
            byte[] returnByte;
            int length = 0;
            bool existsNextPlayer = true;
            do
            {
                i++;
                if (i == pBytes.Length)
                    existsNextPlayer = false;

            } while (i < pBytes.Length && pBytes[i] != NEWLINE_DELIMITER);

            length = i - pOffset - 1;
            returnByte = new byte[length];

            for (i = 0, tempOffset = pOffset + 1; i < length; i++, tempOffset++) // i = 1; skips first character which is newline
            {
                returnByte[i] = pBytes[tempOffset];
            }
            pPlayerBytes = returnByte;

            return existsNextPlayer;
        }

        public static KeyValuePair<string, string>[] GetSettings(byte[] pBytes, int pOffset, out int pLength)
        { 
            var serverSettings = new List<KeyValuePair<string, string>>();
            bool existsNextSetting = true;
            
            int byteCounter = pLength = pOffset; // 1 to skip first slash.
            while (existsNextSetting)
            { 
                // \key\value 
                string key = string.Empty;
                string value = string.Empty;
                bool onValue = false;

                StringBuilder sb = new StringBuilder();
                for(byteCounter = byteCounter + 1; ; byteCounter++)
                {
                    if (byteCounter >= pBytes.Length)
                        break;

                    if (pBytes[byteCounter] == SLASH_DELIMITER && !onValue)
                    {
                        key = sb.ToString();
                        sb = new StringBuilder();
                        onValue = true;
                        continue;
                    }
                    else if (pBytes[byteCounter] == SLASH_DELIMITER)
                    {
                        value = sb.ToString();
                        existsNextSetting = true;
                        break;
                    }
                    else if (pBytes[byteCounter] == NEWLINE_DELIMITER)
                    {
                        value = sb.ToString();
                        existsNextSetting = false;
                        break;
                    }
                    sb.Append((char)pBytes[byteCounter]);
                    
                }
                serverSettings.Add(new KeyValuePair<string, string>(key, value));
                pLength = byteCounter;
            }

            return serverSettings.ToArray();
        }
    }
}
