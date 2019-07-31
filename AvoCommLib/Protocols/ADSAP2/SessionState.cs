using System;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ADSAP2
        {
            [Flags]
            public enum SessionState
            {
                NotShareable = 1,
                NotStealthable = 2
            }
        }
    }
}