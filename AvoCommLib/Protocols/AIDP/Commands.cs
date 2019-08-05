using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using AvoCommLib.Protocols.Base;
using Lextm.SharpSnmpLib;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace AIDP
        {
            [Command("AIDP", (byte)CommandTypes.DiscoverRequest, AlwaysAddEOFField = true)]
            public class DiscoverRequest : BaseCommand
            {
                [CommandField(1)]
                public byte[] Version { get; set; }
                [CommandField(2)]
                public IPAddress Subnet { get; set; }
                [CommandField(3)]
                public byte? CIDR { get; set; }
            }

            [Command("AIDP", (byte)CommandTypes.DiscoverResponse)]
            public class DiscoverResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16), Required = true)]
                public Enums.Models ModelID { get; set; }
                [CommandField(2, Required = true)]
                public byte[] MACAddress { get; set; }
                [CommandField(3, Required = true)]
                public IPAddress IPAddress { get; set; }
                [CommandField(4, Required = true)]
                public IPAddress SubnetAddress { get; set; }
                [CommandField(5, Required = true)]
                public IPAddress GatewayAddress { get; set; }
                [CommandField(6)]
                public string Hostname { get; set; }
                [CommandField(7)]
                public byte? Mode { get; set; }
            }

            public abstract class IPRequest : BaseCommand
            {
                [CommandField(1)]
                public byte[] MACAddress { get; set; }
                [CommandField(2)]
                public IPAddress IPAddress { get; set; }
                [CommandField(3)]
                public IPAddress SubnetAddress { get; set; }
                [CommandField(4)]
                public IPAddress GatewayAddress { get; set; }
                [CommandField(5)]
                public byte? Mode { get; set; }
            }

            [Command("AIDP", (byte)CommandTypes.TestIPRequest)]
            public class TestIPRequest : IPRequest { }

            [Command("AIDP", (byte)CommandTypes.SetIPRequest)]
            public class SetIPRequest : IPRequest { }

            public abstract class IPResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16))]
                public Result? Result { get; set; }
            }

            [Command("AIDP", (byte)CommandTypes.TestIPResponse)]
            public class TestIPResponse : IPResponse { }

            [Command("AIDP", (byte)CommandTypes.SetIPResponse)]
            public class SetIPResponse : IPResponse { }

            public abstract class SNMPRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public List<Variable> Variables { get; set; }
                public Variable Variable { get { return Variables.FirstOrDefault(); } set { Variables = new List<Variable>{ value }; } }
            }

            [Command("AIDP", (byte)CommandTypes.SNMPGetRequest)]
            public class SNMPGetRequest : SNMPRequest {}

            [Command("AIDP", (byte)CommandTypes.SNMPGetNextRequest)]
            public class SNMPGetNextRequest : SNMPRequest {}

            public abstract class SNMPResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16), Required = true)]
                public bool HasError { get; set; }
                [CommandField(2, SerializeAs = typeof(UInt16), Required = true)]
                public Result Result { get; set; }

                [CommandField(3, Required = true)]
                public List<Variable> Variables { get; set; }
                public Variable Variable { get { return Variables.First(); } set { Variables = new List<Variable>{ value }; } }
            }

            [Command("AIDP", (byte)CommandTypes.SNMPGetResponse)]
            public class SNMPGetResponse : SNMPResponse {}

            [Command("AIDP", (byte)CommandTypes.SNMPGetNextResponse)]
            public class SNMPGetNextResponse : SNMPResponse {}
        }
    }
}
