using System.Collections.Generic;

namespace AvoCommLib
{
    namespace Protocols
    {
        namespace Base
        {
            public interface ICommand
            {
                FieldCollection Fields { get; }

                bool IsValid { get; }
            }
        }
    }
}