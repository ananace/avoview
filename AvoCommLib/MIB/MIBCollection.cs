using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Lextm.SharpSnmpLib;

namespace AvoCommLib
{
    namespace MIB
    {
        public static class MIBCollection
        {
            static Dictionary<ObjectIdentifier, MIBNode> _Oids = new Dictionary<ObjectIdentifier, MIBNode>();

            static ObjectIdentifier RootOid { get { return new ObjectIdentifier("1.3"); } }
            public static IMIBNode Root { get { return GetNode(RootOid); } }

            public static IReadOnlyList<MIBNode> RegisteredMIBs { get { return _Oids.Values.ToList(); } }

            public static void EnsureRoot()
            {
                if (_Oids.ContainsKey(RootOid))
                    return;

                var oids = new List<Tuple<string,string>>{
                    new Tuple<string,string>("1.3", "org"),
                    new Tuple<string,string>("1.3.6", "dod"),
                    new Tuple<string,string>("1.3.6.1", "internet"),
                    new Tuple<string,string>("1.3.6.1.4", "private"),
                    new Tuple<string,string>("1.3.6.1.4.1", "enterprises"),
                    new Tuple<string,string>("1.3.6.1.2", "mgmt"),
                };

                foreach (var pair in oids) 
                {
                    var oid = new ObjectIdentifier(pair.Item1);
                    var node = new MIBNode {
                        Name = pair.Item2,
                        Oid = oid
                    };

                    if (pair.Item1.Count(c => c == '.') > 1)
                    {
                        var parentOid = new ObjectIdentifier(string.Join(".", pair.Item1.Split('.').Take(pair.Item1.Count(c => c == '.'))));
                        var parent = _Oids[parentOid];
                        if (parent != null)
                        {
                            parent.AddChild(node);
                            node.Parent = parent;
                        }
                    }

                    _Oids[oid] = node;
                }
            }

            public static bool LoadXML(string Path)
            {
                EnsureRoot();

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

                if (!ParseNode(null, firstNode, rootOid))
                    return false;

                return true;
            }

            public static bool HasOid(ObjectIdentifier input)
            {
                return _Oids.ContainsKey(input);
            }

            public static MIBNode GetNode(ObjectIdentifier input)
            {
                return _Oids[input];
            }

            public static string GetNameFromOid(ObjectIdentifier input)
            {
                if (!_Oids.ContainsKey(input))
                    return null;
                return _Oids[input].Name;
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
                        parts.Add(_Oids[curOid].Name);
                    else if (includeNumerical)
                        parts.Add(oidParts[i].ToString());
                }

                if (parts.Any())
                    return string.Join(".", parts);
                return string.Join(".", oidParts);
            }

            static bool ParseNode(MIBNode parent, XmlNode node, ObjectIdentifier baseOid)
            {
                var oidParts = baseOid.ToNumerical();
                var oid = ObjectIdentifier.Create(oidParts, uint.Parse(node.Attributes.GetNamedItem("id").Value));

                var name = node.Attributes.GetNamedItem("name").Value;

                var mnode = new MIBNode {
                    Name = name,
                    Oid = oid,
                    Parent = parent
                };

                _Oids[oid] = mnode;

                if (parent != null)
                    parent.AddChild(mnode);

                foreach (var child in node.ChildNodes.OfType<XmlNode>().Where((n) => n.Name == "symbol"))
                    mnode.AddValueMapping(int.Parse(child.Attributes.GetNamedItem("value").Value), child.Attributes.GetNamedItem("name").Value);

                foreach (var child in node.ChildNodes.OfType<XmlNode>().Where((n) => n.Name == "node"))
                {
                    if (!ParseNode(mnode, child, oid))
                        return false;
                }

                return true;
            }
        }
    }
}
