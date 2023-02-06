using Org.BouncyCastle.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServersDataAggregation.Query
{
    internal class NameHelper
    {
        const int AFK_LENGTH = 4;
        private static readonly byte[] AFK = new byte[] { (byte)' ', (byte)'A', (byte)'F', (byte)'K' };
        private static readonly byte[] RED_AFK = new byte[] { (byte)' ', 193, 198, 203 };
        public static byte[] ChkRemoveAfk (byte[] nameRaw)
        {
            if (nameRaw.Length > AFK_LENGTH)
            {
                for (int i = nameRaw.Length - AFK_LENGTH, j = 0; j < AFK_LENGTH; i++, j++)
                {
                    if (AFK[j] != nameRaw[i])
                    {
                        break;
                    }
                    if (i == nameRaw.Length - 1)
                    {
                        var newName = new Byte[nameRaw.Length - AFK_LENGTH];
                        Buffer.BlockCopy(nameRaw, 0, newName, 0, newName.Length);
                        return newName;
                    }
                }
                for (int i = nameRaw.Length - AFK_LENGTH, j = 0; j < AFK_LENGTH; i++, j++)
                {
                    if (RED_AFK[j] != nameRaw[i])
                    {
                        break;
                    }
                    if (i == nameRaw.Length - 1)
                    {
                        var newName = new Byte[nameRaw.Length - AFK_LENGTH];
                        Buffer.BlockCopy(nameRaw, 0, newName, 0, newName.Length);
                        return newName;
                    }
                }
            }
            return nameRaw;
        }
    }
}
