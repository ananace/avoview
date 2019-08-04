namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ASMP
        {
            public enum CommandTypes
            {
                LoginRequest = 1,
                LogoutRequest = 2,
                SNMPGetRequest = 16,
                SNMPGetNextRequest = 17,
                SNMPSetRequest = 18,
                OpenWriteRequest = 32,
                WriteRequest = 33,
                CloseWriteRequest = 34,
                OpenReadRequest = 35,
                ReadRequest = 36,
                CloseReadRequest = 37,
                HeartbeatRequest = 38,
                SessionSetupRequest = 48,
                VersionRequest = 49,

                LoginResponse = 128 | LoginRequest,
                LogoutResponse = 128 | LogoutRequest,
                SNMPGetResponse = 128 | SNMPGetRequest,
                SNMPGetNextResponse = 128 | SNMPGetNextRequest,
                SNMPSetResponse = 128 | SNMPSetRequest,
                OpenWriteResponse = 128 | OpenWriteRequest,
                WriteResponse = 128 | WriteRequest,
                CloseWriteResponse = 128 | CloseWriteRequest,
                OpenReadResponse = 128 | OpenReadRequest,
                ReadResponse = 128 | ReadRequest,
                CloseReadResponse = 128 | CloseReadRequest,
                HeartbeatResponse = 128 | HeartbeatRequest,
                SessionSetupResponse = 128 | SessionSetupRequest,
                VersionResponse = 128 | VersionRequest,

                Broadcast = 255,
            }
        }
    }
}
