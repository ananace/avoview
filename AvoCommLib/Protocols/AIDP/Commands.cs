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
                public byte[] Version { get { return Fields.First(1)?.FieldData; } set { Fields.Set(new CommandField(1, value)); } }
                [CommandField(2)]
                public IPAddress Subnet { get { return Fields.First(2)?.AsIPAddress(); } set { Fields.Set(new CommandField(2, value)); } }
                [CommandField(3)]
                public byte? CIDR { get { return Fields.First(3)?.AsByte(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(3, value.Value));
                    else
                        Fields.Remove(3);
                } }
            }

            [Command("AIDP", (byte)CommandTypes.DiscoverResponse)]
            public class DiscoverResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16))]
                public Enums.Models ModelID { get { return (Enums.Models)Fields.First(1)?.AsUInt16(); } }
                [CommandField(2)]
                public byte[] MACAddress { get { return Fields.First(2)?.FieldData; } }
                [CommandField(3)]
                public IPAddress IPAddress { get { return Fields.First(3)?.AsIPAddress(); } }
                [CommandField(4)]
                public IPAddress SubnetAddress { get { return Fields.First(4)?.AsIPAddress(); } }
                [CommandField(5)]
                public IPAddress GatewayAddress { get { return Fields.First(5)?.AsIPAddress(); } }
                [CommandField(6)]
                public string Hostname { get { return Fields.First(6)?.AsString(); } }
                [CommandField(7)]
                public byte? Mode { get { return Fields.First(7)?.AsByte(); } }
            }

            public abstract class IPRequest : BaseCommand
            {
                [CommandField(1)]
                public byte[] MACAddress { get { return Fields.First(1)?.FieldData; } set { Fields.Set(new CommandField(1, value)); } }
                [CommandField(2)]
                public IPAddress IPAddress { get { return Fields.First(2)?.AsIPAddress(); } set { Fields.Set(new CommandField(2, value)); } }
                [CommandField(3)]
                public IPAddress SubnetAddress { get { return Fields.First(3)?.AsIPAddress(); } set { Fields.Set(new CommandField(3, value)); } }
                [CommandField(4)]
                public IPAddress GatewayAddress { get { return Fields.First(4)?.AsIPAddress(); } set { Fields.Set(new CommandField(4, value)); } }
                [CommandField(5)]
                public byte? Mode { get { return Fields.First(5)?.AsByte(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(5, value.Value));
                    else
                        Fields.Remove(5);
                } }
            }

            [Command("AIDP", (byte)CommandTypes.TestIPRequest)]
            public class TestIPRequest : IPRequest { }

            [Command("AIDP", (byte)CommandTypes.SetIPRequest)]
            public class SetIPRequest : IPRequest { }

            public abstract class IPResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16))]
                public Result? Result { get { return (Result?)Fields.First(1)?.AsUInt16(); } }
            }

            [Command("AIDP", (byte)CommandTypes.TestIPResponse)]
            public class TestIPResponse : IPResponse { }

            [Command("AIDP", (byte)CommandTypes.SetIPResponse)]
            public class SetIPResponse : IPResponse { }

            public abstract class SNMPRequest : BaseCommand
            {
                [CommandField(1)]
                public IEnumerable<Variable> Variables { get {
                    return Fields.Where(1).Select(f => f.AsVarBind());
                } set {
                    Fields.Remove(1);
                    Fields.Fields.AddRange(value.Select(v => new CommandField(1, v)));
                } }
                public Variable Variable { get { return Variables.First(); } set { Variables = new[] { value }; } }
            }

            [Command("AIDP", (byte)CommandTypes.SNMPGetRequest)]
            public class SNMPGetRequest : SNMPRequest {}

            [Command("AIDP", (byte)CommandTypes.SNMPGetNextRequest)]
            public class SNMPGetNextRequest : SNMPRequest {}

            public abstract class SNMPResponse : BaseCommand
            {
                [CommandField(1)]
                public bool HasError { get { return Fields.First(1)?.AsUInt16() != 0; } }
                [CommandField(2, SerializeAs = typeof(UInt16))]
                public Result? Result { get { return (Result?)Fields.First(2)?.AsUInt16(); } }

                [CommandField(3)]
                public IEnumerable<Variable> Variables { get {
                    return Fields.Where(3).Select(f => f.AsVarBind());
                } }
                public Variable Variable { get { return Variables.First(); } }
            }

            [Command("AIDP", (byte)CommandTypes.SNMPGetResponse)]
            public class SNMPGetResponse : SNMPResponse {}

            [Command("AIDP", (byte)CommandTypes.SNMPGetNextResponse)]
            public class SNMPGetNextResponse : SNMPResponse {}
        }
    }
}
