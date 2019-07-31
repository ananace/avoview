namespace AvoCommLib
{
    namespace Protocols
    {
        namespace AIDP
        {
            public enum CommandTypes
            {
                DiscoverRequest = 1,
                TestIPRequest = 2,
                SetIPRequest = 3,
                SNMPGetRequest = 16,
                SNMPGetNextRequest = 17,

                DiscoverResponse = 128 | DiscoverRequest,
                TestIPResponse = 128 | TestIPRequest,
                SetIPResponse = 128 | SetIPRequest,
                SNMPGetResponse = 128 | SNMPGetRequest,
                SNMPGetNextResponse = 128 | SNMPGetNextRequest
            }
        }
    }
}