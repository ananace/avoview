namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ADSAP2
        {
            public enum Status
            {
                NoError = 0,
                Error = 256,
                ResourceBusy,
                ResourceBlocked,
                Unauthorized,
                InTrustAll,
                ResourceUnavailable,
                TooManyPending,
                ResourceUpgrading,
                TooManySessions
            }
        }
    }
}