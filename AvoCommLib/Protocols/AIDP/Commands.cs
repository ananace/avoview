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
                public byte[] Version { get { return Fields.First(1)?.FieldData; } set { Fields.Set(new CommandField(1, value)); } }
                public IPAddress Subnet { get { return Fields.First(2)?.AsIPAddress(); } set { Fields.Set(new CommandField(2, value)); } }
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
                public Enums.Models ModelID { get { return (Enums.Models)Fields.First(1)?.AsUInt16(); } }
                public byte[] MACAddress { get { return Fields.First(2)?.FieldData; } }
                public IPAddress IPAddress { get { return Fields.First(3)?.AsIPAddress(); } }
                public IPAddress SubnetAddress { get { return Fields.First(4)?.AsIPAddress(); } }
                public IPAddress GatewayAddress { get { return Fields.First(5)?.AsIPAddress(); } }
                public string Hostname { get { return Fields.First(6)?.AsString(); } }
                public byte? Mode { get { return Fields.First(7)?.AsByte(); } }
            }

            public abstract class IPRequest : BaseCommand
            {
                public byte[] MACAddress { get { return Fields.First(1)?.FieldData; } set { Fields.Set(new CommandField(1, value)); } }
                public IPAddress IPAddress { get { return Fields.First(2)?.AsIPAddress(); } set { Fields.Set(new CommandField(2, value)); } }
                public IPAddress SubnetAddress { get { return Fields.First(3)?.AsIPAddress(); } set { Fields.Set(new CommandField(3, value)); } }
                public IPAddress GatewayAddress { get { return Fields.First(4)?.AsIPAddress(); } set { Fields.Set(new CommandField(4, value)); } }
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
                public Result? Result { get { return (Result?)Fields.First(1)?.AsUInt16(); } }
            }

            [Command("AIDP", (byte)CommandTypes.TestIPResponse)]
            public class TestIPResponse : IPResponse { }

            [Command("AIDP", (byte)CommandTypes.SetIPResponse)]
            public class SetIPResponse : IPResponse { }

            public abstract class SNMPRequest : BaseCommand
            {
                public Variable Variable { get { return Fields.First(1)?.AsVarBind(); } set { Fields.Set(new CommandField(1, value)); } }
                public IEnumerable<Variable> Variables { get {
                    return Fields.Where(1).Select(f => f.AsVarBind());
                } set {
                    Fields.Remove(1);
                    Fields.Fields.AddRange(value.Select(v => new CommandField(1, v)));
                } }
            }

            [Command("AIDP", (byte)CommandTypes.SNMPGetRequest)]
            public class SNMPGetRequest : SNMPRequest {}

            [Command("AIDP", (byte)CommandTypes.SNMPGetNextRequest)]
            public class SNMPGetNextRequest : SNMPRequest {}

            public abstract class SNMPResponse : BaseCommand
            {
                public bool HasError { get { return Fields.First(1)?.AsUInt16() != 0; } }
                public Result? Result { get { return (Result?)Fields.First(2)?.AsUInt16(); } }

                public Variable Variable { get { return Fields.First(3)?.AsVarBind(); } }
                public IEnumerable<Variable> Variables { get {
                    return Fields.Where(3).Select(f => f.AsVarBind());
                } }
            }

            [Command("AIDP", (byte)CommandTypes.SNMPGetResponse)]
            public class SNMPGetResponse : SNMPResponse {}

            [Command("AIDP", (byte)CommandTypes.SNMPGetNextResponse)]
            public class SNMPGetNextResponse : SNMPResponse {}
        }
    }
}
