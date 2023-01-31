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
        private static readonly byte[] AFK = new byte[] { (byte)' ', (byte)'A', (byte)'F', (byte)'K' };
        public static byte[] ChkRemoveAfk (byte[] nameRaw)
        {
            if (nameRaw.Length > AFK.Length)
            {
                for (int i = nameRaw.Length - AFK.Length, j = 0; j < AFK.Length; i++, j++)
                {
                    if (AFK[j] != nameRaw[i])
                    {
                        break;
                    }
                    if (i == nameRaw.Length - 1)
                    {
                        var newName = new Byte[nameRaw.Length - AFK.Length];
                        Buffer.BlockCopy(nameRaw, 0, newName, 0, newName.Length);
                        return newName;
                    }
                }
            }
            return nameRaw;
        }
    }
}
