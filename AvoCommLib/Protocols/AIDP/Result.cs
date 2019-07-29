namespace AvoCommLib
{
    namespace Protocols
    {
        namespace AIDP
        {
            public enum Result
            {
                NoError = 0,
                TooBig,
                NoSuchName,
                BadValue,
                ReadOnly,
                GenericError,
                InvalidRequest,
                ApplianceError,
                IPAddressAlreadyExists
            }
        }
    }
}