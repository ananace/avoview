namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ASMP
        {
            public enum Result
            {
                NoError = 0,
                TooBig,
                NoSuchName,
                BadValue,
                ReadOnly,
                GenericError,
                NoAccess,

                InvalidRequest = 256,
                ApplianceError,
                OutOfMemory,
                UnknownUserPassword,
                MaxSessionsActive,
                NoUserLoggedIn,
                FileAlreadyOpenForWrite,
                FileNotOpenForWrite,
                FileAlreadyOpenForRead,
                FileNotOpenForRead,
                FileNotOpen,
                FileNotFound,
                LockedUser,
                UserAccessDenied,
                AuthServerError,
                BadCACert
            }
        }
    }
}