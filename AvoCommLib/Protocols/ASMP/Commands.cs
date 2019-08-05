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
                public string Username { get; set; }
                [CommandField(2, Required = true)]
                public string Password { get; set; }
            }

            public abstract class ResultResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16), Required = true)]
                public Result Result { get; set; }
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
                public Variable[] Variables { get; set; }
                public Variable Variable { get { return Variables.FirstOrDefault(); } set { Variables = new[] { value }; } }
            }

            [Command("ASMP", (byte)CommandTypes.SNMPGetRequest)]
            public class SNMPGetRequest : SNMPRequest {}

            [Command("ASMP", (byte)CommandTypes.SNMPGetNextRequest)]
            public class SNMPGetNextRequest : SNMPRequest {}

            [Command("ASMP", (byte)CommandTypes.SNMPSetRequest)]
            public class SNMPSetRequest : SNMPRequest
            {
                [CommandField(2)]
                public string Username { get; set; }
            }

            public abstract class SNMPResponse : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(UInt16), Required = true)]
                public bool HasError { get; set; }
                [CommandField(2, SerializeAs = typeof(UInt16), Required = true)]
                public Result Result { get; set; }

                [CommandField(3, Required = true)]
                public Variable[] Variables { get; set; }
                public Variable Variable { get { return Variables.FirstOrDefault(); } set { Variables = new[] { value }; } }
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
                public string FileName { get; set; }
                [CommandField(2)]
                public long? FileSize { get; set; }
                [CommandField(3)]
                public int? FileType { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.OpenWriteResponse)]
            public class OpenWriteResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.WriteResponse)]
            public class WriteRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public int BlockNumber { get; set; }
                [CommandField(2)]
                public byte[] BlockData { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.WriteResponse)]
            public class WriteResponse : ResultResponse
            {
                [CommandField(2)]
                public int? BlockNumber { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.CloseWriteRequest)]
            public class CloseWriteRequest : BaseCommand {}

            [Command("ASMP", (byte)CommandTypes.CloseWriteResponse)]
            public class CloseWriteResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.OpenReadRequest)]
            public class OpenReadRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public string FileName { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.OpenReadResponse)]
            public class OpenReadResponse : ResultResponse
            {
                [CommandField(2)]
                public long? FileSize { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.ReadRequest)]
            public class ReadRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public int BlockNumber { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.ReadResponse)]
            public class ReadResponse : ResultResponse
            {
                [CommandField(2)]
                public int? BlockNumber { get; set; }
                [CommandField(3)]
                public byte[] BlockData { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.CloseReadRequest)]
            public class CloseReadRequest : BaseCommand {}

            [Command("ASMP", (byte)CommandTypes.CloseReadResponse)]
            public class CloseReadResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.HeartbeatRequest)]
            public class HeartbeatRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public short Interval { get; set; } = 60;
                [CommandField(2, Required = true)]
                public short Timeout { get; set; } = 120;
            }

            [Command("ASMP", (byte)CommandTypes.HeartbeatResponse)]
            public class HeartbeatResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.SessionSetupRequest)]
            public class SessionSetupRequest : BaseCommand
            {
                [CommandField(1, SerializeAs = typeof(Int16))]
                public ConnectionType? ConnectionType { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.SessionSetupResponse)]
            public class SessionSetupResponse : ResultResponse {}

            [Command("ASMP", (byte)CommandTypes.VersionRequest)]
            public class VersionRequest : BaseCommand
            {
                [CommandField(1, Required = true)]
                public string ClientVersion { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.VersionResponse)]
            public class VersionResponse : ResultResponse
            {
                [CommandField(1, Required = true)]
                public string Version { get; set; }
            }

            [Command("ASMP", (byte)CommandTypes.Broadcast)]
            public class Broadcast : BaseCommand
            {
                [CommandField(1, Required = true)]
                public short Type { get; set; }
                [CommandField(2, Required = true)]
                public string Message { get; set; }
                [CommandField(3, Required = true)]
                public short MessageCode { get; set; }
            }
        }
    }
}
