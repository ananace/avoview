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
        namespace ASMP
        {
            [Command("ASMP", (byte)CommandTypes.LoginRequest)]
            public class LoginRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public string Username { get { return Fields.First(1)?.AsString(); } set { Fields.Set(new CommandField(1, value)); } }
                [CommandField(2, Required = true)]
                public string Password { get { return Fields.First(2)?.AsString(); } set { Fields.Set(new CommandField(2, value)); } }
            }

            public abstract class ResultResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16))]
                public Result? Result { get { return (Result?)Fields.First(1)?.AsUInt16(); } }
            }

            [Command("ASMP", (byte)CommandTypes.LoginResponse)]
            public class LoginResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.LogoutRequest)]
            public class LogoutRequest : BaseCommand {}

            [Command("ASMP", (byte)CommandTypes.LoginResponse)]
            public class LogoutResponse : ResultResponse {}

            public abstract class SNMPRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public IEnumerable<Variable> Variables { get {
                    return Fields.Where(1).Select(f => f.AsVarBind());
                } set {
                    Fields.Remove(1);
                    Fields.Fields.AddRange(value.Select(v => new CommandField(1, v)));
                } }

                public Variable Variable { get { return Variables.First(); } set { Variables = new[] { value }; } }
            }

            [Command("ASMP", (byte)CommandTypes.SNMPGetRequest)]
            public class SNMPGetRequest : SNMPRequest {}

            [Command("ASMP", (byte)CommandTypes.SNMPGetNextRequest)]
            public class SNMPGetNextRequest : SNMPRequest {}

            [Command("ASMP", (byte)CommandTypes.SNMPSetRequest)]
            public class SNMPSetRequest : SNMPRequest
            {
                [CommandField(2)]
                public string Username { get { return Fields.First(2)?.AsString(); } set { Fields.Set(new CommandField(2, value)); } }
            }

            public abstract class SNMPResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16))]
                public bool HasError { get { return Fields.First(1)?.AsUInt16() != 0; } }
                [CommandField(2, SerializeAs = typeof(UInt16))]
                public Result? Result { get { return (Result?)Fields.First(2)?.AsUInt16(); } }

                [CommandField(3)]
                public IEnumerable<Variable> Variables { get {
                    return Fields.Where(3).Select(f => f.AsVarBind());
                } }
                public Variable Variable { get { return Variables.First(); } }
            }

            [Command("ASMP", (byte)CommandTypes.SNMPGetResponse)]
            public class SNMPGetResponse : SNMPResponse {}

            [Command("ASMP", (byte)CommandTypes.SNMPGetNextResponse)]
            public class SNMPGetNextResponse : SNMPResponse {}

            [Command("ASMP", (byte)CommandTypes.SNMPSetResponse)]
            public class SNMPSetResponse : SNMPResponse {}

            [Command("ASMP", (byte)CommandTypes.OpenWriteRequest)]
            public class OpenWriteRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public string FileName { get { return Fields.First(1)?.AsString(); } set { Fields.Set(new CommandField(1, value)); } }
                [CommandField(2)]
                public long? FileSize { get { return Fields.First(2)?.AsInt64(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(2, value.Value));
                    else
                        Fields.Remove(2);
                } }
                [CommandField(3)]
                public int? FileType { get { return Fields.First(3)?.AsInt32(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(3, value.Value));
                    else
                        Fields.Remove(3);
                } }
            }

            [Command("ASMP", (byte)CommandTypes.OpenWriteResponse)]
            public class OpenWriteResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.WriteResponse)]
            public class WriteRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public int? BlockNumber { get { return Fields.First(1)?.AsInt32(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(1, value.Value));
                    else
                        Fields.Remove(1);
                } }
                [CommandField(2)]
                public byte[] BlockData { get { return Fields.First(2)?.FieldData; } set { Fields.Set(new CommandField(2, value)); } }
            }

            [Command("ASMP", (byte)CommandTypes.WriteResponse)]
            public class WriteResponse : ResultResponse
            {
                [CommandField(2)]
                public int? BlockNumber { get { return Fields.First(2)?.AsInt32(); } }
            }

            [Command("ASMP", (byte)CommandTypes.CloseWriteRequest)]
            public class CloseWriteRequest : BaseCommand {}

            [Command("ASMP", (byte)CommandTypes.CloseWriteResponse)]
            public class CloseWriteResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.OpenReadRequest)]
            public class OpenReadRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public string FileName { get { return Fields.First(1)?.AsString(); } set { Fields.Set(new CommandField(1, value)); } }
            }

            [Command("ASMP", (byte)CommandTypes.OpenReadResponse)]
            public class OpenReadResponse : ResultResponse
            {
                [CommandField(2)]
                public long? FileSize { get { return Fields.First(2)?.AsInt64(); } }
            }

            [Command("ASMP", (byte)CommandTypes.ReadRequest)]
            public class ReadRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public int? BlockNumber { get { return Fields.First(1)?.AsInt32(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(1, value.Value));
                    else
                        Fields.Remove(1);
                } }
            }

            [Command("ASMP", (byte)CommandTypes.ReadResponse)]
            public class ReadResponse : ResultResponse
            {
                [CommandField(2)]
                public int? BlockNumber { get { return Fields.First(2)?.AsInt32(); } }
                [CommandField(3)]
                public byte[] BlockData { get { return Fields.First(3)?.FieldData; } }
            }

            [Command("ASMP", (byte)CommandTypes.CloseReadRequest)]
            public class CloseReadRequest : BaseCommand {}

            [Command("ASMP", (byte)CommandTypes.CloseReadResponse)]
            public class CloseReadResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.HeartbeatRequest)]
            public class HeartbeatRequest : BaseCommand
            {
                public HeartbeatRequest()
                {
                    // Default values
                    Interval = 60;
                    Timeout = 120;
                }

                [CommandField(1, Required = true)]
                public short? Interval { get { return Fields.First(1)?.AsInt16(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(1, value.Value));
                    else
                        Fields.Remove(1);
                } }
                [CommandField(2, Required = true)]
                public short? Timeout { get { return Fields.First(2)?.AsInt16(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(2, value.Value));
                    else
                        Fields.Remove(2);
                } }
            }

            [Command("ASMP", (byte)CommandTypes.HeartbeatResponse)]
            public class HeartbeatResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.SessionSetupRequest)]
            public class SessionSetupRequest : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(Int16))]
                public ConnectionType? ConnectionType { get { return (ConnectionType?)Fields.First(1)?.AsInt16(); } set { Fields.Set(new CommandField(1, (Int16)value)); } }
            }

            [Command("ASMP", (byte)CommandTypes.SessionSetupResponse)]
            public class SessionSetupResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.VersionRequest)]
            public class VersionRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public string ClientVersion { get { return Fields.First(1)?.AsString(); } set { Fields.Set(new CommandField(1, value)); } }
            }

            [Command("ASMP", (byte)CommandTypes.VersionResponse)]
            public class VersionResponse : ResultResponse
            {
                [CommandField(1)]
                public string Version { get { return Fields.First(1)?.AsString(); } }
            }

            [Command("ASMP", (byte)CommandTypes.Broadcast)]
            public class Broadcast : BaseCommand
            {
                [CommandField(1, Required = true)]
                public short? Type { get { return Fields.First(1)?.AsInt16(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(1, value.Value));
                    else
                        Fields.Remove(1);
                } }
                [CommandField(2, Required = true)]
                public string Message { get { return Fields.First(2)?.AsString(); } set { Fields.Set(new CommandField(2, value)); } }
                [CommandField(3, Required = true)]
                public short? MessageCode { get { return Fields.First(3)?.AsInt16(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(3, value.Value));
                    else
                        Fields.Remove(3);
                } }
            }
        }
    }
}
