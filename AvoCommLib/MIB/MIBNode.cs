using System.Collections.Generic;
using Lextm.SharpSnmpLib;

namespace AvoCommLib
{
    namespace MIB
    {
        public class MIBNode : IMIBNode
        {
            public string Name { get; set; }
            public ObjectIdentifier Oid { get; set; }

            public IMIBNode Parent { get; set; }
            public IReadOnlyList<IMIBNode> Children { get { return _children; } }
            List<MIBNode> _children = new List<MIBNode>();

            public MIBAccessibility Accessibility { get; set; } = MIBAccessibility.NotApplicable;
            public IReadOnlyDictionary<int, string> ValueMappings { get { return _valueMappings; } }
            Dictionary<int,string> _valueMappings = new Dictionary<int,string>();

            public void AddChild(MIBNode child)
            {
                _children.Add(child);
            }

            public void AddValueMapping(int key, string name)
            {
                _valueMappings[key] = name;
            }
        }
    }
}
