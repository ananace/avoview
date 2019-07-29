using System;
using System.IO;
using System.Linq;
using System.Net;
using AvoCommLib.Enums;
using AvoCommLib.Util;

namespace AvoCommLib
{
    public class Appliance
    {
        public string Hostname { get { if (string.IsNullOrEmpty(_hostname)) { SNMPGetHostname(); } return _hostname; } private set { _hostname = value; } }
        public Models Model { get; private set; }
        public IPAddress IPAddress { get { return _ipAddress; } set { _ipAddress = value; AIDPSession.Target.Address = value; } }
        public IPAddress GatewayAddress { get; private set; }
        public IPAddress SubnetAddress { get; private set; }
        public byte[] MACAddress { get; private set; }
        public int SubnetLength { get; private set; }
        public byte Mode { get; private set; }

        IPAddress _ipAddress;
        string _hostname;

        public Protocols.AIDP.Session AIDPSession { get; private set; } = new Protocols.AIDP.Session();

        public void Discover()
        {
            AIDPSession.Target = new IPEndPoint(IPAddress, 3211);

            var res = AIDPSession.SendRequest(new Protocols.AIDP.DiscoverRequest());
            res.Wait();

            ParseDiscoveryData(res.Result as Protocols.AIDP.DiscoverResponse);
        }

        void SNMPGetHostname()
        {
            var ret = AIDPSession.SendRequest(new Protocols.AIDP.SNMPGetRequest{ Variable = new Lextm.SharpSnmpLib.Variable(new Lextm.SharpSnmpLib.ObjectIdentifier("1.3.6.1.2.1.1.5.0")) });
            ret.Wait();
            _hostname = (ret.Result as Protocols.AIDP.SNMPGetResponse).Variable.Data.ToString();
        }

        void ParseDiscoveryData(Protocols.AIDP.DiscoverResponse data)
        {
            Model = data.ModelID;
            MACAddress = data.MACAddress;
            IPAddress = data.IPAddress;
            SubnetAddress = data.SubnetAddress;
            GatewayAddress = data.GatewayAddress;
            Hostname = data.Hostname;
            if (data.Mode.HasValue)
                Mode = data.Mode.Value;
        }
    }
}
