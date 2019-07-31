using System;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace ADSAP2
        {
            [Flags]
            public enum SessionDetails
            {
                AccessRights = 1,
                AuthToken = 2
            }
        }
    }
}