using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Lextm.SharpSnmpLib;

namespace AvoCommLib
{
    namespace Util
    {
        public static class MIBCollection
        {
            static Dictionary<ObjectIdentifier, string> _Oids = new Dictionary<ObjectIdentifier, string>();

            public static bool LoadXML(string Path)
            {
                XmlDocument doc;

                using (var fstream = new FileStream(Path, FileMode.Open))
                using (var read = XmlReader.Create(fstream))
                {
                    doc = new XmlDocument();
                    doc.Load(read);
                }

                var root = doc.SelectSingleNode("./mib");

                var mibName = root.Attributes.GetNamedItem("name").Value;
                var rootOid = new ObjectIdentifier(root.Attributes.GetNamedItem("rootOid").Value);

                var firstNode = root.SelectSingleNode("./node");

                if (!ParseNode(firstNode, rootOid))
                    return false;

                return true;
            }

            public static bool HasOid(ObjectIdentifier input)
            {
                return _Oids.ContainsKey(input);
            }

            public static string GetNameFromOid(ObjectIdentifier input)
            {
                if (!_Oids.ContainsKey(input))
                    return null;
                return _Oids[input];
            }

            public static string GetFullNameFromOid(ObjectIdentifier input, bool includeNumerical = true)
            {
                var oidParts = input.ToNumerical();
                List<string> parts = new List<string>();

                if (includeNumerical)
                    parts.AddRange(oidParts.Take(2).Select((p) => p.ToString()));

                for (int i = 1; i < oidParts.Length; ++i)
                {
                    var curOid = new ObjectIdentifier(oidParts.Take(i + 1).ToArray());

                    if (_Oids.ContainsKey(curOid))
                        parts.Add(_Oids[curOid]);
                    else if (includeNumerical)
                        parts.Add(oidParts[i].ToString());
                }

                if (parts.Any())
                    return string.Join(".", parts);
                return string.Join(".", oidParts);
            }

            static bool ParseNode(XmlNode node, ObjectIdentifier baseOid)
            {
                var oidParts = baseOid.ToNumerical();
                var oid = ObjectIdentifier.Create(oidParts, uint.Parse(node.Attributes.GetNamedItem("id").Value));

                var name = node.Attributes.GetNamedItem("name").Value;

                _Oids[oid] = name;

                foreach (var child in node.ChildNodes.OfType<XmlNode>().Where((n) => n.Name == "node"))
                {
                    if (!ParseNode(child, oid))
                        return false;
                }

                return true;
            }
        }
    }
}
