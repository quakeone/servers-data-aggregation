using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Query.Games.Common;

public static class NameUtils
{    public static string PlayerBytesToString(byte[] pPlayerName)
    {
        StringBuilder sb = new StringBuilder();
        foreach (byte b in pPlayerName)
        {
            sb.Append(ConvertByteToChar(b));
        }
        return sb.ToString();
    }

    private static char ConvertByteToChar(byte pByte)
    {
        if (pByte >= 0x20 && pByte < 0x80)
            return (char)pByte;
        if (pByte >= 0xA0 && pByte <= 0xFF)
            return (char)(pByte - 128);

        switch (pByte)
        {
            case 0x0d:
            case 0x8d:
                return '>';
            case 0x00:
            case 0x05:
            case 0x0e:
            case 0x0f:
            case 0x0c:
            case 0x85:
            case 0x8e:
            case 0x8f:
            case 0x9c:
                return (char)183; // Middle dot, product of sign
            case 0x10:
            case 0x90:
                return '[';
            case 0x11:
            case 0x91:
                return ']';
            case 0x12:
            case 0x92:
                return '0';
            case 0x13:
            case 0x93:
                return '1';
            case 0x14:
            case 0x94:
                return '2';
            case 0x15:
            case 0x95:
                return '3';
            case 0x96:
            case 0x16:
                return '4';
            case 0x17:
            case 0x97:
                return '5';
            case 0x18:
            case 0x98:
                return '6';
            case 0x19:
            case 0x99:
                return '7';
            case 0x1A:
            case 0x9A:
                return '8';
            case 0x1B:
            case 0x9B:
                return '9';
        }
        return ' ';
    }
}
