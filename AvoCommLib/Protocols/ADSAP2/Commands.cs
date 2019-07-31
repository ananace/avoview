using System.Collections.Generic;
using System.Linq;
using System.Net;
using AvoCommLib.Protocols.Base;
using Lextm.SharpSnmpLib;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ADSAP2
        {
            [Command("ADSAP2", (byte)CommandTypes.SetCertificatesRequest)]
            public class SetCertificatesRequest : BaseCommand
            {
                public byte[] SystemCertificate { get { return Fields.First(1)?.FieldData; } set { Fields.Set(new CommandField(1, value)); } }
                public byte[] ApplianceCertificate { get { return Fields.First(2)?.FieldData; } set { Fields.Set(new CommandField(2, value)); } }
                public byte[] PrivateKey { get { return Fields.First(3)?.FieldData; } set { Fields.Set(new CommandField(3, value)); } }
            }

            public class StatusResponse : BaseCommand
            {
                public Status? Status { get { return (Status?)Fields.First(1)?.AsInt16(); } }
            }

            [Command("ADSAP2", (byte)CommandTypes.SetCertificatesResponse)]
            public class SetCertificatesResponse : StatusResponse { }

            [Command("ADSAP2", (byte)CommandTypes.ClearCertificatesRequest)]
            public class ClearCertificatesRequest : BaseCommand { }

            [Command("ADSAP2", (byte)CommandTypes.ClearCertificatesResponse)]
            public class ClearCertificatesResponse : StatusResponse { }

            [Command("ADSAP2", (byte)CommandTypes.PreauthorizeRequest)]
            public class PreauthorizeRequest : BaseCommand
            {
                public byte[] SessionCertificate { get { return Fields.First(1)?.FieldData; } set { Fields.Set(new CommandField(1, value)); } }
            }

            [Command("ADSAP2", (byte)CommandTypes.PreauthorizeResponse)]
            public class PreauthorizeResponse : StatusResponse
            {
                public string Username { get { return Fields.First(2)?.AsString(); } }
                public byte? PreemptionLevel { get { return Fields.First(3)?.AsByte(); } }
                public SessionState? SessionState { get { return (SessionState?)Fields.First(4)?.AsByte(); } }
            }

            [Command("ADSAP2", (byte)CommandTypes.AuthorizeRequest)]
            public class AuthorizeRequest : BaseCommand
            {
                public byte[] SessionCert { get { return Fields.First(1)?.FieldData; } set { Fields.Set(new CommandField(1, value)); } }

                public string Username { get { return Fields.First(2)?.AsString(); } set {
                    if (value == null)
                        Fields.Remove(2);
                    else
                        Fields.Set(new CommandField(2, value));
                } }
                public string Password { get { return Fields.First(3)?.AsString(); } set {
                    if (value == null)
                        Fields.Remove(3);
                    else
                        Fields.Set(new CommandField(3, value));
                } }
                public byte[] PublicKey { get { return Fields.First(4)?.FieldData; } set { Fields.Set(new CommandField(4, value)); } }
                public string SessionMode { get { return Fields.First(5)?.AsString(); } set {
                    if (value == null)
                        Fields.Remove(5);
                    else
                        Fields.Set(new CommandField(5, value));
                } }
                public long? AuthenticationToken { get { return Fields.First(6)?.AsInt64(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(6, value.Value));
                    else
                        Fields.Remove(6);
                } }
                public SessionDetails? Details { get { return (SessionDetails?)Fields.First(127)?.AsInt16(); } set {
                    if (value.HasValue)
                        Fields.Set(new CommandField(127, (short)value.Value));
                    else
                        Fields.Remove(127);
                } }
                public IEnumerable<string> AddressLevels { get {
                    for (byte i = 0; i < 128; ++i)
                    {
                        if (!Fields.HasField((byte)(128 + i)))
                            continue;

                        yield return Fields.First((byte)(128 + i)).AsString();
                    }
                } }

                public override bool IsValid { get {
                    if (Fields.HasField(1) && Fields.Count() > 1)
                        return false; // Session cert includes everything

                    if (!Fields.HasField(2))
                        return false; // No username given

                    if (!Fields.HasField(3) && !Fields.HasField(4))
                        return false; // Neither password nor public key given
                    if (Fields.HasField(3) && Fields.HasField(4))
                        return false; // Both password and public key given

                    return true;
                } }
            }

            [Command("ADSAP2", (byte)CommandTypes.AuthorizeResponse)]
            public class AuthorizeResponse : StatusResponse
            {
                public byte? PreemptionLevel { get { return Fields.First(2)?.AsByte(); } }
                public string ApplianceRole { get { return Fields.First(3)?.AsString(); } }
                public string ApplianceRights { get { return Fields.First(4)?.AsString(); } }
                public string TargetRights { get { return Fields.First(5)?.AsString(); } }
                public long? AuthenticationToken { get { return Fields.First(6)?.AsInt64(); } }
            }
        }
    }
}