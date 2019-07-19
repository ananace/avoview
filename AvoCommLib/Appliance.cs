using System;
using System.Net;

namespace AvoCommLib
{
    public class Appliance
    {
        public string Hostname { get; private set; }
        public Enums.Models Model { get; private set; }
        public IPAddress IPAddress { get; private set; }
        public IPAddress GatewayAddress { get; private set; }
        public IPAddress SubnetAddress { get; private set; }
        public byte[] MacAddress { get; private set; }
        public int SubnetLength { get; private set; }

        void Discover()
        {
            var aidp = new Protocols.AIDP(IPAddress);
            aidp.Discover();
        }
    }
}
