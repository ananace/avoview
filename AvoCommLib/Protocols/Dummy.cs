using System;
using System.Linq;
using System.Threading.Tasks;

namespace AvoCommLib
{
    namespace Protocols
    {
        public class Dummy : Base
        {
            protected override async Task<int> WriteData(byte[] data)
            {
                Console.WriteLine("Writing data:");
                Console.WriteLine(String.Join(" ", data.Select((b) => b.ToString("X"))));

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
