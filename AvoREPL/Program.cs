using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lextm.SharpSnmpLib;

namespace AvoREPL
{
    class Program
    {
        static void Main(string[] args)
        {
            AvoCommLib.MIB.MIBCollection.EnsureRoot();

            for (int i = 0; i < args.Length; ++i)
            {
                var arg = args[i];

                if (arg == "-m")
                {
                    var mibsFolder = args[++i];

                    var files = Directory.GetFiles(mibsFolder, "*.xml");
                    foreach (var file in files)
                    {
                        var cBefore = AvoCommLib.MIB.MIBCollection.RegisteredMIBs.Count;
                        AvoCommLib.MIB.MIBCollection.LoadXML(file);
                        var cAfter = AvoCommLib.MIB.MIBCollection.RegisteredMIBs.Count;

                        Console.WriteLine($"Loaded {cAfter - cBefore} MIB Nodes from {file}");
                    }
                }
            }

            while (true)
            {
                Console.Write("> ");
                var cmd = Console.ReadLine();
                if (string.IsNullOrEmpty(cmd))
                    break;

                var parts = cmd.Split();

                try
                {
                    switch (parts.First().ToLower())
                    {
                        case "discover":
                            {
                                var ip = System.Net.IPAddress.Parse(parts[1].ToLower());
                                var appliance = new AvoCommLib.Appliance();
                                appliance.IPAddress = ip;
                                appliance.Discover();

                                // Console.WriteLine(appliance.SystemName);
                            }
                            break;

                        case "snmpnext":
                            goto case "snmp";
                        case "snmp":
                            {
                                var ip = System.Net.IPAddress.Parse(parts[1].ToLower());
                                var vbl = new List<Variable>();

                                foreach (var part in parts.Skip(2))
                                {
                                    var oid = new ObjectIdentifier(part);
                                    Console.WriteLine($"Adding Oid: {oid}");
                                    var vb = new Variable(oid);

                                    vbl.Add(vb);
                                }

                                var aidp = new AvoCommLib.Protocols.AIDP(ip);

                                Task<List<Variable>> ret;
                                if (parts.First().ToLower() == "snmp")
                                    ret = aidp.SnmpGet(vbl);
                                else
                                    ret = aidp.SnmpGetNext(vbl);

                                ret.Wait();

                                foreach (var vb in ret.Result)
                                    Console.WriteLine($"{AvoCommLib.MIB.MIBCollection.GetFullNameFromOid(vb.Id)}: {vb.Data}");
                            }
                            break;
                        
                        case "loadmib":
                            {
                                var cBefore = AvoCommLib.MIB.MIBCollection.RegisteredMIBs.Count;
                                AvoCommLib.MIB.MIBCollection.LoadXML(parts[1]);
                                var cAfter = AvoCommLib.MIB.MIBCollection.RegisteredMIBs.Count;

                                Console.WriteLine($"Loaded {cAfter - cBefore} MIB Nodes from {parts[1]}");
                            }
                            break;

                        case "oid":
                            {
                                foreach (var part in parts.Skip(1))
                                {
                                    var oid = new ObjectIdentifier(part);
                                    Console.WriteLine($"Oid {oid}:");

                                    var node = AvoCommLib.MIB.MIBCollection.GetNode(oid);

                                    Console.WriteLine($"Type: {node.Type}");
                                    Console.WriteLine($"Parent: {node.Parent.Oid}");
                                    Console.WriteLine($"Full name: {AvoCommLib.MIB.MIBCollection.GetFullNameFromOid(oid)}");

                                    if (node.Type == AvoCommLib.MIB.MIBType.Int)
                                    {
                                        var mappings = (node as AvoCommLib.MIB.MIBNode).ValueMappings;
                                        Console.WriteLine("Values:");
                                        foreach (var kv in mappings)
                                            Console.WriteLine($"- {kv.Key} == {kv.Value}");
                                    }

                                    Console.WriteLine("Children:");
                                    foreach (var child in node.Children)
                                        Console.WriteLine($"- {child.Oid} - {child.Name}");

                                    Console.WriteLine();
                                }
                            }
                            break;

                        default:
                            Console.WriteLine($"Unknown input: \"{parts.First()}\"");
                            goto case "help";

                        case "help":
                            Console.WriteLine("Available commands:");
                            Console.WriteLine();
                            Console.WriteLine("- help");
                            Console.WriteLine("- discover IP");
                            Console.WriteLine("- loadmib FILE.XML");
                            Console.WriteLine("- oid OID...");
                            Console.WriteLine("- snmp IP OID...");
                            Console.WriteLine("- snmpnext IP OID...");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            Console.WriteLine();
        }
    }
}
