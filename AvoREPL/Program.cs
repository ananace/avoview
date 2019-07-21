using System;

namespace AvoREPL
{
    class Program
    {
        static void Main(string[] args)
        {
            var appliance = new AvoCommLib.Appliance();
            appliance.IPAddress = System.Net.IPAddress.Parse("172.31.0.245");

            appliance.Discover();

            Console.WriteLine("Hello World!");
        }
    }
}
