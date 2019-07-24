using System.Collections.Generic;
using Lextm.SharpSnmpLib;

namespace AvoCommLib
{
    namespace MIB
    {
        public interface IMIBNode
        {
            string Name { get; }
            ObjectIdentifier Oid { get; }

            IMIBNode Parent { get; }
            IReadOnlyList<IMIBNode> Children { get; }

            MIBAccessibility Accessibility { get; }
        }
    }
}
