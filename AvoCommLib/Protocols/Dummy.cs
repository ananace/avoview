using System;
using System.Linq;
using System.Threading.Tasks;

namespace AvoCommLib
{
    namespace Protocols
    {
        public class Dummy_ : Base_
        {
            protected override async Task<int> WriteData(byte[] data)
            {
                await Task.Yield();
                return data.Length;
            }

            protected override async Task<byte[]> ReadData()
            {
                await Task.Yield();
                return new byte[] {};
            }
        }
    }
}
