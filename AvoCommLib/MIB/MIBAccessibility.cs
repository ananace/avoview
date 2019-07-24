using System;

namespace AvoCommLib
{
    namespace MIB
    {
        [Flags]
        public enum MIBAccessibility
        {
            NotApplicable = 0,

            Read          = 1,
            ReadOnly      = Read,

            Write         = 2,
            WriteOnly     = Write,

            ReadWrite     = Read | Write
        }
    }
}
