namespace AvoCommLib
{
    namespace Enums
    {
        public enum AIDPError
        {
            Ok = 0,
            TooBig,
            NoSuchName,
            BadValue,
            ReadOnly,
            Generic,
            InvalidRequest,
            ApplianceError,
            IPInUse,

            Timeout = 4096,
            BadReply,
            IOError
        }
    }
}