using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerService.Models
{
    public class Crc16
    {
        public static ushort Calculate(byte[] data)
        {
            const ushort poly = 0xA001;
            ushort crc = 0xFFFF;

            foreach (byte b in data)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    bool lsb = (crc & 1) == 1;
                    crc >>= 1;
                    if (lsb) crc ^= poly;
                }
            }
            return crc;
        }
    }
}
