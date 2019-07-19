using System;

namespace AvoREPL
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new AvoCommLib.Protocols.AIDP(new System.Net.IPEndPoint(new System.Net.IPAddress(new byte[] { 172, 31, 0, 245}), 3211));
            test.Discover();

            Console.WriteLine("Hello World!");
        }
    }
}
